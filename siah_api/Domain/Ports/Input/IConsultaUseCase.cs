using SiahApi.Application.DTOs.Consulta;

namespace SiahApi.Domain.Ports.Input;

public interface IConsultaUseCase
{
    Task<IEnumerable<ConsultaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request);
}
