using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IEspecialidadeRepository
{
    Task<IEnumerable<Especialidade>> ListarAsync();
    Task<IEnumerable<Medico>> ListarMedicosPorEspecialidadeAsync(Guid especialidadeId);
}
