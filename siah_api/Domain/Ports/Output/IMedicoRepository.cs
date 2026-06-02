using SiahApi.Domain.Entities;
using SiahApi.Application.DTOs.Medico;

namespace SiahApi.Domain.Ports.Output;

public interface IMedicoRepository
{
    Task<IEnumerable<Medico>> ListarAsync(MedicoFiltros filtros);
    Task<IEnumerable<Medico>> ListarFavoritosPorUsuarioAsync(Guid userId);
    Task<Medico?> ObterPorIdAsync(Guid medicoId);
    Task<IEnumerable<(string Data, string Horario, bool Disponivel)>> ObterDisponibilidadeAsync(Guid medicoId, string? dataInicio, string? dataFim, Guid? especialidadeId);
    Task<IEnumerable<(string Avaliador, string? Comentario, decimal Nota, DateTime Data)>> ObterAvaliacoesAsync(Guid medicoId);
    Task FavoritarAsync(Guid userId, Guid medicoId);
    Task DesfavoritarAsync(Guid userId, Guid medicoId);
    Task<IEnumerable<Medico>> BuscarAsync(string termo);
}
