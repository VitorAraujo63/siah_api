using SiahApi.Domain.Entities;
using SiahApi.Application.DTOs.Historico;

namespace SiahApi.Domain.Ports.Output;

public interface IHistoricoRepository
{
    Task<IEnumerable<Agendamento>> ListarHistoricoPorUsuarioAsync(Guid userId, HistoricoFiltros filtros);
    Task<Agendamento?> ObterHistoricoPorIdAsync(Guid atendimentoId);
    Task<IEnumerable<Agendamento>> ListarRecentesPorUsuarioAsync(Guid userId);
}
