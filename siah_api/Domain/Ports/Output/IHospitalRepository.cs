using SiahApi.Application.DTOs.Hospital;

namespace SiahApi.Domain.Ports.Output;

public interface IHospitalRepository
{
    Task<IEnumerable<HospitalResponse>> ListarAsync();
}
