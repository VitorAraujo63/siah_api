using SiahApi.Application.DTOs.Fila;

namespace SiahApi.Domain.Ports.Input;

public interface IFilaUseCase
{
    Task<EmitirSenhaResponse> ValidarTotemAsync(ValidarTotemRequest request);
    Task<SenhaAtivaResponse> ObterSenhaAtivaAsync(Guid userId);
    Task<StatusSenhaResponse> ObterStatusAsync(Guid ticketId);
    Task ConfirmarPresencaAsync(Guid userId, Guid ticketId);
}
