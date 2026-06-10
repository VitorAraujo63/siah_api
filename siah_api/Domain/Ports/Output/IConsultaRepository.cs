using SiahApi.Application.DTOs.Consulta;

namespace SiahApi.Domain.Ports.Output;

public interface IConsultaRepository
{
    Task<IEnumerable<ConsultaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request);
}
