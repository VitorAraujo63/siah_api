using SiahApi.Application.DTOs.Consulta;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class ConsultaUseCase : IConsultaUseCase
{
    private readonly IConsultaRepository _consultaRepository;

    public ConsultaUseCase(IConsultaRepository consultaRepository)
    {
        _consultaRepository = consultaRepository;
    }

    public async Task<IEnumerable<ConsultaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        return await _consultaRepository.ListarPorCpfAsync(cpf, pesquisa);
    }

    public async Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request)
    {
        return await _consultaRepository.CriarAsync(request);
    }
}
