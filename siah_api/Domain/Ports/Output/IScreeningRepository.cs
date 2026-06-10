using SiahApi.Application.DTOs.Screening;

namespace SiahApi.Domain.Ports.Output;

public interface IScreeningRepository
{
    Task<IEnumerable<ScreeningResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<ScreeningResponse> CriarAsync(Guid idUsuario, CriarScreeningRequest request);
    Task<Guid?> ObterIdUsuarioPorCpfAsync(string cpf);
}
