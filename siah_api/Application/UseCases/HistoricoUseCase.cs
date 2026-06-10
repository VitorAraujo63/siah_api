using SiahApi.Application.DTOs.Historico;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class HistoricoUseCase : IHistoricoUseCase
{
    private readonly IHistoricoRepository _historicoRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IAuthRepository _authRepository;

    public HistoricoUseCase(IHistoricoRepository historicoRepository, IMedicoRepository medicoRepository, IAuthRepository authRepository)
    {
        _historicoRepository = historicoRepository;
        _medicoRepository = medicoRepository;
        _authRepository = authRepository;
    }

    public async Task<IEnumerable<HistoricoResponse>> ListarAsync(string cpf, HistoricoFiltros filtros)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var atendimentos = await _historicoRepository.ListarHistoricoPorUsuarioAsync(userId, filtros);
        return await MapearListaAsync(atendimentos);
    }

    public async Task<HistoricoResponse> ObterPorIdAsync(string cpf, Guid atendimentoId)
    {
        var atendimento = await _historicoRepository.ObterHistoricoPorIdAsync(atendimentoId)
            ?? throw new KeyNotFoundException("Atendimento não encontrado.");

        return await MapearAsync(atendimento);
    }

    public async Task<IEnumerable<HistoricoResponse>> ListarRecentesAsync(string cpf)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var atendimentos = await _historicoRepository.ListarRecentesPorUsuarioAsync(userId);
        return await MapearListaAsync(atendimentos);
    }

    private async Task<Guid> ResolverUserIdAsync(string cpf)
    {
        var paciente = await _authRepository.ObterPorCpfAsync(cpf)
            ?? throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");
        return paciente.Id;
    }

    private async Task<IEnumerable<HistoricoResponse>> MapearListaAsync(
        IEnumerable<Domain.Entities.Agendamento> atendimentos)
    {
        var tasks = atendimentos.Select(a => MapearAsync(a));
        return await Task.WhenAll(tasks);
    }

    private async Task<HistoricoResponse> MapearAsync(Domain.Entities.Agendamento atendimento)
    {
        var medico = await _medicoRepository.ObterPorIdAsync(atendimento.MedicoId);

        return new HistoricoResponse
        {
            Id = atendimento.Id,
            Data = atendimento.Data,
            Horario = atendimento.Horario,
            Status = atendimento.Status,
            Medico = medico is null ? null : new MedicoHistoricoDto
            {
                Id = medico.Id,
                Nome = medico.Nome,
                FotoUrl = medico.FotoUrl
            },
            Especialidade = medico?.Especialidade
        };
    }
}
