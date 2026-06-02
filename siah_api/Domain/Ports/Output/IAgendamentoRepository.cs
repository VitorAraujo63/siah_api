using SiahApi.Domain.Entities;
using SiahApi.Application.DTOs.Agendamento;

namespace SiahApi.Domain.Ports.Output;

public interface IAgendamentoRepository
{
    Task<IEnumerable<Agendamento>> ListarPorUsuarioAsync(Guid userId, AgendamentoFiltros filtros);
    Task<IEnumerable<Agendamento>> ListarProximosPorUsuarioAsync(Guid userId);
    Task<Agendamento?> ObterPorIdAsync(Guid agendamentoId);
    Task<bool> HorarioDisponivelAsync(Guid medicoId, string data, string horario);
    Task<Agendamento> CriarAsync(Agendamento agendamento);
    Task<Agendamento> AtualizarAsync(Agendamento agendamento);
}
