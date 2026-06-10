using SiahApi.Application.DTOs.PacientDetail;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class PacientDetailUseCase : IPacientDetailUseCase
{
    private readonly IPacientDetailRepository _pacientDetailRepository;

    public PacientDetailUseCase(IPacientDetailRepository pacientDetailRepository)
    {
        _pacientDetailRepository = pacientDetailRepository;
    }

    public async Task<PacientDetailResponse?> ObterPorCpfAsync(string cpf)
    {
        return await _pacientDetailRepository.ObterPorCpfAsync(cpf);
    }
}
