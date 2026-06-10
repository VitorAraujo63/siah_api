using SiahApi.Application.DTOs.PacientDetail;

namespace SiahApi.Domain.Ports.Input;

public interface IPacientDetailUseCase
{
    Task<PacientDetailResponse?> ObterPorCpfAsync(string cpf);
}
