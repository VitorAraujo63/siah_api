using SiahApi.Application.DTOs.Medico;

namespace SiahApi.Domain.Ports.Input;

public interface IMedicoUseCase
{
    Task<IEnumerable<MedicoResponse>> ListarAsync(MedicoFiltros filtros);
    Task<IEnumerable<MedicoResponse>> ListarFavoritosAsync(Guid userId);
    Task<MedicoResponse> ObterPorIdAsync(Guid medicoId);
    Task<DisponibilidadeResponse> ObterDisponibilidadeAsync(Guid medicoId, DisponibilidadeFiltros filtros);
    Task<IEnumerable<AvaliacaoResponse>> ObterAvaliacoesAsync(Guid medicoId);
    Task FavoritarAsync(Guid userId, Guid medicoId);
    Task DesfavoritarAsync(Guid userId, Guid medicoId);
    Task<IEnumerable<MedicoResponse>> BuscarAsync(string termo);
}
