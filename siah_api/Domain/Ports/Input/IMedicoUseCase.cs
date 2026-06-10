using SiahApi.Application.DTOs.Medico;

namespace SiahApi.Domain.Ports.Input;

public interface IMedicoUseCase
{
    Task<IEnumerable<MedicoResponse>> ListarAsync(MedicoFiltros filtros);
    Task<IEnumerable<MedicoResponse>> ListarFavoritosAsync(string cpf);
    Task<MedicoResponse> ObterPorIdAsync(Guid medicoId);
    Task<DisponibilidadeResponse> ObterDisponibilidadeAsync(Guid medicoId, DisponibilidadeFiltros filtros);
    Task<IEnumerable<AvaliacaoResponse>> ObterAvaliacoesAsync(Guid medicoId);
    Task FavoritarAsync(string cpf, Guid medicoId);
    Task DesfavoritarAsync(string cpf, Guid medicoId);
    Task<IEnumerable<MedicoResponse>> BuscarAsync(string termo);
}
