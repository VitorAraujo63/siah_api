using SiahApi.Application.DTOs.Hospital;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class HospitalUseCase : IHospitalUseCase
{
    private readonly IHospitalRepository _hospitalRepository;

    public HospitalUseCase(IHospitalRepository hospitalRepository)
    {
        _hospitalRepository = hospitalRepository;
    }

    public async Task<IEnumerable<HospitalResponse>> ListarAsync()
    {
        return await _hospitalRepository.ListarAsync();
    }
}
