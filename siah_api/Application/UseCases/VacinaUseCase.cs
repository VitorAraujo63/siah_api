using SiahApi.Application.DTOs.Vacina;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class VacinaUseCase : IVacinaUseCase
{
    private readonly IVacinaRepository _vacinaRepository;

    public VacinaUseCase(IVacinaRepository vacinaRepository)
    {
        _vacinaRepository = vacinaRepository;
    }

    public async Task<IEnumerable<VacinaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        return await _vacinaRepository.ListarPorCpfAsync(cpf, pesquisa);
    }

    public async Task<VacinaResponse> CriarAsync(CriarVacinaRequest request)
    {
        return await _vacinaRepository.CriarAsync(request);
    }

    public async Task<VacinaResponse?> AtualizarAsync(Guid id, AtualizarVacinaRequest request)
    {
        return await _vacinaRepository.AtualizarAsync(id, request);
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        return await _vacinaRepository.RemoverAsync(id);
    }
}
