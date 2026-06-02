using SiahApi.Application.DTOs.Perfil;
using Microsoft.AspNetCore.Http;

namespace SiahApi.Domain.Ports.Input;

public interface IPerfilUseCase
{
    Task<PerfilResponse> ObterAsync(Guid userId);
    Task<PerfilResponse> AtualizarAsync(Guid userId, AtualizarPerfilRequest request);
    Task<PerfilResponse> AtualizarDadosBasicosAsync(Guid userId, AtualizarDadosBasicosRequest request);
    Task<AtualizarFotoResponse> AtualizarFotoAsync(Guid userId, IFormFile photo);
    Task RemoverFotoAsync(Guid userId);
    Task SolicitarExclusaoAsync(Guid userId);
}
