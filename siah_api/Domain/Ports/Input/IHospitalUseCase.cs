using SiahApi.Application.DTOs.Hospital;

namespace SiahApi.Domain.Ports.Input;

public interface IHospitalUseCase
{
    Task<IEnumerable<HospitalResponse>> ListarAsync();
}
