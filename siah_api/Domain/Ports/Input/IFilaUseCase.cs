using SiahApi.Application.DTOs.Fila;

namespace SiahApi.Domain.Ports.Input;

public interface IFilaUseCase
{
    Task<EmitirSenhaResponse> ValidarTotemAsync(ValidarTotemRequest request);
    Task<SenhaAtivaResponse> ObterSenhaAtivaAsync(string cpf);
    Task<StatusSenhaResponse> ObterStatusAsync(long ticketId);
    Task FinalizarAtendimentoAsync(long ticketId);
}
