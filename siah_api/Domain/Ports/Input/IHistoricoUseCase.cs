using SiahApi.Application.DTOs.Historico;

namespace SiahApi.Domain.Ports.Input;

public interface IHistoricoUseCase
{
    Task<IEnumerable<HistoricoResponse>> ListarAsync(Guid userId, HistoricoFiltros filtros);
    Task<HistoricoResponse> ObterPorIdAsync(Guid userId, Guid atendimentoId);
    Task<IEnumerable<HistoricoResponse>> ListarRecentesAsync(Guid userId);
}
