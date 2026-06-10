using SiahApi.Application.DTOs.Historico;

namespace SiahApi.Domain.Ports.Input;

public interface IHistoricoUseCase
{
    Task<IEnumerable<HistoricoResponse>> ListarAsync(string cpf, HistoricoFiltros filtros);
    Task<HistoricoResponse> ObterPorIdAsync(string cpf, Guid atendimentoId);
    Task<IEnumerable<HistoricoResponse>> ListarRecentesAsync(string cpf);
}
