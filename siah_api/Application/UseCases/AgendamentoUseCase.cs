using SiahApi.Application.DTOs.Agendamento;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class AgendamentoUseCase : IAgendamentoUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IAuthRepository _authRepository;

    public AgendamentoUseCase(IAgendamentoRepository agendamentoRepository, IMedicoRepository medicoRepository, IAuthRepository authRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _medicoRepository = medicoRepository;
        _authRepository = authRepository;
    }

    public async Task<IEnumerable<AgendamentoResponse>> ListarAsync(string cpf, AgendamentoFiltros filtros)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var agendamentos = await _agendamentoRepository.ListarPorUsuarioAsync(userId, filtros);
        return await MapearListaAsync(agendamentos);
    }

    public async Task<IEnumerable<AgendamentoResponse>> ListarProximosAsync(string cpf)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var agendamentos = await _agendamentoRepository.ListarProximosPorUsuarioAsync(userId);
        return await MapearListaAsync(agendamentos);
    }

    public async Task<AgendamentoResponse> ObterPorIdAsync(string cpf, Guid agendamentoId)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(agendamentoId)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        if (agendamento.IdUsuario != userId)
            throw new UnauthorizedAccessException("Acesso negado.");

        return await MapearAsync(agendamento);
    }

    public async Task<AgendamentoResponse> AgendarAsync(string cpf, AgendarRequest request)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var disponivel = await _agendamentoRepository.HorarioDisponivelAsync(request.DoctorId, request.Date, request.TimeSlot);
        if (!disponivel)
            throw new InvalidOperationException("SLOT_UNAVAILABLE");

        var agendamento = new Agendamento
        {
            IdUsuario = userId,
            MedicoId = request.DoctorId,
            EspecialidadeId = request.SpecialtyId,
            Data = request.Date,
            Horario = request.TimeSlot,
            Observacoes = request.Notes,
            PlanoId = request.InsuranceId,
            Status = "scheduled"
        };

        var criado = await _agendamentoRepository.CriarAsync(agendamento);
        return await MapearAsync(criado);
    }

    public async Task<AgendamentoResponse> ReagendarAsync(string cpf, Guid agendamentoId, ReagendarRequest request)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(agendamentoId)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        if (agendamento.IdUsuario != userId)
            throw new UnauthorizedAccessException("Acesso negado.");

        agendamento.Data = request.Date;
        agendamento.Horario = request.TimeSlot;

        var atualizado = await _agendamentoRepository.AtualizarAsync(agendamento);
        return await MapearAsync(atualizado);
    }

    public async Task<AgendamentoResponse> CancelarAsync(string cpf, Guid agendamentoId, CancelarRequest request)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(agendamentoId)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        if (agendamento.IdUsuario != userId)
            throw new UnauthorizedAccessException("Acesso negado.");

        agendamento.Status = "cancelled";
        agendamento.MotivoCancelamento = request.Reason;
        agendamento.TipoCancelamento = request.CancelType;
        agendamento.CanceladoEm = DateTime.UtcNow;
        agendamento.ReembolsoElegivel = true;

        var atualizado = await _agendamentoRepository.AtualizarAsync(agendamento);
        return await MapearAsync(atualizado);
    }

    private async Task<Guid> ResolverUserIdAsync(string cpf)
    {
        var paciente = await _authRepository.ObterPorCpfAsync(cpf)
            ?? throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");
        return paciente.Id;
    }

    private async Task<IEnumerable<AgendamentoResponse>> MapearListaAsync(IEnumerable<Agendamento> agendamentos)
    {
        var tasks = agendamentos.Select(a => MapearAsync(a));
        return await Task.WhenAll(tasks);
    }

    private async Task<AgendamentoResponse> MapearAsync(Agendamento agendamento)
    {
        var medico = await _medicoRepository.ObterPorIdAsync(agendamento.MedicoId);

        return new AgendamentoResponse
        {
            AppointmentId = agendamento.Id,
            Date = agendamento.Data,
            Time = agendamento.Horario,
            Status = agendamento.Status,
            QueueNumber = agendamento.NumeroDaSenha,
            RefundEligible = agendamento.ReembolsoElegivel,
            CancelledAt = agendamento.CanceladoEm,
            Doctor = medico is null ? null : new MedicoAgendamentoDto
            {
                Nome = medico.Nome,
                Especialidade = medico.Especialidade,
                FotoUrl = medico.FotoUrl
            }
        };
    }
}
