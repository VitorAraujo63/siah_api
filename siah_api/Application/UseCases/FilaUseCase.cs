using SiahApi.Application.DTOs.Fila;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class FilaUseCase : IFilaUseCase
{
    private readonly IFilaRepository _filaRepository;

    public FilaUseCase(IFilaRepository filaRepository)
    {
        _filaRepository = filaRepository;
    }

    public async Task<EmitirSenhaResponse> ValidarTotemAsync(ValidarTotemRequest request)
    {
        var posicao = await _filaRepository.ObterProximaPosicaoAsync(request.DepartmentId);
        var tempoEspera = posicao * 7;

        var senha = new SenhaAtendimento
        {
            IdPaciente = request.PatientId,
            TotemId = request.TotemId,
            TipoServico = request.ServiceType,
            DepartamentoId = request.DepartmentId,
            NumeroSenha = $"A-{posicao:D3}",
            PosicaoNaFila = posicao,
            TempoEsperaMinutos = tempoEspera,
            Status = "waiting"
        };

        var emitida = await _filaRepository.EmitirSenhaAsync(senha);

        return new EmitirSenhaResponse
        {
            Sucesso = true,
            Data = new EmitirSenhaDataDto
            {
                TicketId = emitida.Id,
                TicketNumber = emitida.NumeroSenha,
                QueuePosition = emitida.PosicaoNaFila,
                EstimatedWaitMinutes = emitida.TempoEsperaMinutos,
                Department = emitida.DepartamentoId.ToString(),
                IssuedAt = emitida.EmitidaEm
            }
        };
    }

    public async Task<SenhaAtivaResponse> ObterSenhaAtivaAsync(Guid userId)
    {
        var senha = await _filaRepository.ObterSenhaAtivaPorUsuarioAsync(userId)
            ?? throw new KeyNotFoundException("Nenhuma senha ativa encontrada.");

        return new SenhaAtivaResponse
        {
            TicketId = senha.Id,
            TicketNumber = senha.NumeroSenha,
            Status = senha.Status,
            QueuePosition = senha.PosicaoNaFila,
            EstimatedWaitMinutes = senha.TempoEsperaMinutos,
            CalledAt = senha.ChamadaEm,
            Department = senha.DepartamentoId.ToString()
        };
    }

    public async Task<StatusSenhaResponse> ObterStatusAsync(Guid ticketId)
    {
        var senha = await _filaRepository.ObterPorIdAsync(ticketId)
            ?? throw new KeyNotFoundException("Senha não encontrada.");

        return new StatusSenhaResponse
        {
            TicketId = senha.Id,
            TicketNumber = senha.NumeroSenha,
            Status = senha.Status,
            QueuePosition = senha.PosicaoNaFila,
            EstimatedWaitMinutes = senha.TempoEsperaMinutos
        };
    }

    public async Task ConfirmarPresencaAsync(Guid userId, Guid ticketId)
    {
        await _filaRepository.AtualizarStatusAsync(ticketId, "in_service");
    }
}
