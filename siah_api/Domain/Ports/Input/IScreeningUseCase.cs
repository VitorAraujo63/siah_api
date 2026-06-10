using SiahApi.Application.DTOs.Screening;

namespace SiahApi.Domain.Ports.Input;

public interface IScreeningUseCase
{
    Task<IEnumerable<ScreeningResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<ScreeningResponse> CriarAsync(CriarScreeningRequest request);
}
