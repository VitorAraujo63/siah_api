using SiahApi.Application.DTOs.Especialidade;

namespace SiahApi.Domain.Ports.Input;

public interface IEspecialidadeUseCase
{
    Task<IEnumerable<EspecialidadeResponse>> ListarAsync();
    Task<IEnumerable<MedicoSummaryResponse>> ListarMedicosPorEspecialidadeAsync(Guid especialidadeId);
}
