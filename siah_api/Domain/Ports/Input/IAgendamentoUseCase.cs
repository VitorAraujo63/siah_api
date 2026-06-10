using SiahApi.Application.DTOs.Agendamento;

namespace SiahApi.Domain.Ports.Input;

public interface IAgendamentoUseCase
{
    Task<IEnumerable<AgendamentoResponse>> ListarAsync(string cpf, AgendamentoFiltros filtros);
    Task<IEnumerable<AgendamentoResponse>> ListarProximosAsync(string cpf);
    Task<AgendamentoResponse> ObterPorIdAsync(string cpf, Guid agendamentoId);
    Task<AgendamentoResponse> AgendarAsync(string cpf, AgendarRequest request);
    Task<AgendamentoResponse> ReagendarAsync(string cpf, Guid agendamentoId, ReagendarRequest request);
    Task<AgendamentoResponse> CancelarAsync(string cpf, Guid agendamentoId, CancelarRequest request);
}
