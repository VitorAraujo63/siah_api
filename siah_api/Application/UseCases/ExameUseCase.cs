using SiahApi.Application.DTOs.Exame;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class ExameUseCase : IExameUseCase
{
    private readonly IExameRepository _exameRepository;

    public ExameUseCase(IExameRepository exameRepository)
    {
        _exameRepository = exameRepository;
    }

    public async Task<IEnumerable<ExameResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        return await _exameRepository.ListarPorCpfAsync(cpf, pesquisa);
    }

    public async Task<ExameResponse> CriarAsync(CriarExameRequest request)
    {
        return await _exameRepository.CriarAsync(request);
    }
}
