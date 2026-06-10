using SiahApi.Application.DTOs.Rag;

namespace SiahApi.Domain.Ports.Input;

public interface IRagUseCase
{
    Task<RagPatientDto?> ObterPacientePorCpfAsync(string cpf);
    Task<IEnumerable<RagConsultationDto>> ListarConsultasPorCpfAsync(string cpf);
}
