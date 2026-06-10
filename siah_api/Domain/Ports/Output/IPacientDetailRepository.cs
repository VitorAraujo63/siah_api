using SiahApi.Application.DTOs.PacientDetail;

namespace SiahApi.Domain.Ports.Output;

public interface IPacientDetailRepository
{
    Task<PacientDetailResponse?> ObterPorCpfAsync(string cpf);
}
