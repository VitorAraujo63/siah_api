using SiahApi.Application.DTOs.Triagem;

namespace SiahApi.Domain.Ports.Input;

public interface ITriagemUseCase
{
    Task<RegistrarTriagemResponse> RegistrarAsync(RegistrarTriagemRequest request);
}
