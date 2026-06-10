using SiahApi.Application.DTOs.Screening;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class ScreeningUseCase : IScreeningUseCase
{
    private readonly IScreeningRepository _screeningRepository;

    public ScreeningUseCase(IScreeningRepository screeningRepository)
    {
        _screeningRepository = screeningRepository;
    }

    public async Task<IEnumerable<ScreeningResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        return await _screeningRepository.ListarPorCpfAsync(cpf, pesquisa);
    }

    public async Task<ScreeningResponse> CriarAsync(CriarScreeningRequest request)
    {
        var idUsuario = await _screeningRepository.ObterIdUsuarioPorCpfAsync(request.Cpf);
        if (idUsuario is null)
            throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");

        return await _screeningRepository.CriarAsync(idUsuario.Value, request);
    }
}
