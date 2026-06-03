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
        var posicao = await _filaRepository.ObterProximaPosicaoAsync(request.ServiceType);
        var tempoEspera = posicao * 7;

        var senha = new SenhaAtendimento
        {
            Cpf = request.PatientCpf,
            NumeroSenha = $"A-{posicao:D3}",
            Especialidade = request.ServiceType,
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
                QueuePosition = posicao,
                EstimatedWaitMinutes = tempoEspera,
                IssuedAt = emitida.CreatedAt
            }
        };
    }

    public async Task<SenhaAtivaResponse> ObterSenhaAtivaAsync(string cpf)
    {
        var senha = await _filaRepository.ObterSenhaAtivaPorUsuarioAsync(cpf)
            ?? throw new KeyNotFoundException("Nenhuma senha ativa encontrada.");

        var posicao = await _filaRepository.ObterProximaPosicaoAsync(senha.Especialidade);

        return new SenhaAtivaResponse
        {
            TicketId = senha.Id,
            TicketNumber = senha.NumeroSenha,
            Status = senha.Status ?? "waiting",
            QueuePosition = posicao,
            EstimatedWaitMinutes = posicao * 7
        };
    }

    public async Task<StatusSenhaResponse> ObterStatusAsync(long ticketId)
    {
        var senha = await _filaRepository.ObterPorIdAsync(ticketId)
            ?? throw new KeyNotFoundException("Senha não encontrada.");

        var posicao = await _filaRepository.ObterProximaPosicaoAsync(senha.Especialidade);

        return new StatusSenhaResponse
        {
            TicketId = senha.Id,
            TicketNumber = senha.NumeroSenha,
            Status = senha.Status ?? "waiting",
            QueuePosition = posicao,
            EstimatedWaitMinutes = posicao * 7
        };
    }

    public async Task ConfirmarPresencaAsync(long ticketId)
    {
        await _filaRepository.AtualizarStatusAsync(ticketId, "in_service");
    }
}
