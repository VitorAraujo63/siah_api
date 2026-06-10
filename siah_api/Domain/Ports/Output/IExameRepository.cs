using SiahApi.Application.DTOs.Exame;

namespace SiahApi.Domain.Ports.Output;

public interface IExameRepository
{
    Task<IEnumerable<ExameResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<ExameResponse> CriarAsync(CriarExameRequest request);
}
