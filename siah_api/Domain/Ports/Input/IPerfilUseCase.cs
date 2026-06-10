using SiahApi.Application.DTOs.Perfil;
using Microsoft.AspNetCore.Http;

namespace SiahApi.Domain.Ports.Input;

public interface IPerfilUseCase
{
    Task<PerfilResponse> ObterAsync(string cpf);
    Task<PerfilResponse> AtualizarAsync(string cpf, AtualizarPerfilRequest request);
    Task<PerfilResponse> AtualizarDadosBasicosAsync(string cpf, AtualizarDadosBasicosRequest request);
    Task<AtualizarFotoResponse> AtualizarFotoAsync(string cpf, IFormFile photo);
    Task RemoverFotoAsync(string cpf);
    Task SolicitarExclusaoAsync(string cpf);
}
