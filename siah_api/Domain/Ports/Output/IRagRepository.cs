using SiahApi.Application.DTOs.Rag;

namespace SiahApi.Domain.Ports.Output;

public interface IRagRepository
{
    Task<RagPatientDto?> ObterPacientePorCpfAsync(string cpf);
    Task<IEnumerable<RagConsultationDto>> ListarConsultasPorCpfAsync(string cpf);
}
