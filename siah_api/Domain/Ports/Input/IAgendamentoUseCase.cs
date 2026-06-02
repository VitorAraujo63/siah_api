using SiahApi.Application.DTOs.Agendamento;

namespace SiahApi.Domain.Ports.Input;

public interface IAgendamentoUseCase
{
    Task<IEnumerable<AgendamentoResponse>> ListarAsync(Guid userId, AgendamentoFiltros filtros);
    Task<IEnumerable<AgendamentoResponse>> ListarProximosAsync(Guid userId);
    Task<AgendamentoResponse> ObterPorIdAsync(Guid userId, Guid agendamentoId);
    Task<AgendamentoResponse> AgendarAsync(Guid userId, AgendarRequest request);
    Task<AgendamentoResponse> ReagendarAsync(Guid userId, Guid agendamentoId, ReagendarRequest request);
    Task<AgendamentoResponse> CancelarAsync(Guid userId, Guid agendamentoId, CancelarRequest request);
}
