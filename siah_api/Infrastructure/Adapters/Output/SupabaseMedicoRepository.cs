using Npgsql;
using SiahApi.Application.DTOs.Medico;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseMedicoRepository : IMedicoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseMedicoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Medico>> ListarAsync(MedicoFiltros filtros)
    {
        var resultado = new List<Medico>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, especialidade FROM profissionais WHERE tipo_profissional ILIKE 'Medico'";

        if (!string.IsNullOrEmpty(filtros.Name))
        {
            cmd.CommandText += " AND nome ILIKE @nome";
            cmd.Parameters.AddWithValue("nome", $"%{filtros.Name}%");
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearMedico(reader));

        return resultado;
    }

    public Task<IEnumerable<Medico>> ListarFavoritosPorUsuarioAsync(Guid userId)
    {
        // Mock: Não há tabela de médicos favoritos no DB atual
        return Task.FromResult<IEnumerable<Medico>>(new List<Medico>());
    }

    public async Task<Medico?> ObterPorIdAsync(Guid medicoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, especialidade FROM profissionais WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", medicoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearMedico(reader);
    }

    public Task<IEnumerable<(string Data, string Horario, bool Disponivel)>> ObterDisponibilidadeAsync(
        Guid medicoId, string? dataInicio, string? dataFim, Guid? especialidadeId)
    {
        return Task.FromResult<IEnumerable<(string, string, bool)>>(new List<(string, string, bool)>());
    }

    public Task<IEnumerable<(string Avaliador, string? Comentario, decimal Nota, DateTime Data)>> ObterAvaliacoesAsync(Guid medicoId)
    {
        // Mock: Não há tabela de avaliações no DB atual
        return Task.FromResult<IEnumerable<(string, string?, decimal, DateTime)>>(new List<(string, string?, decimal, DateTime)>());
    }

    public Task FavoritarAsync(Guid userId, Guid medicoId)
    {
        // Mock
        return Task.CompletedTask;
    }

    public Task DesfavoritarAsync(Guid userId, Guid medicoId)
    {
        // Mock
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Medico>> BuscarAsync(string termo)
    {
        var resultado = new List<Medico>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, nome, especialidade
            FROM profissionais
            WHERE tipo_profissional ILIKE 'Medico' AND (nome ILIKE @termo OR especialidade ILIKE @termo)";
        cmd.Parameters.AddWithValue("termo", $"%{termo}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearMedico(reader));

        return resultado;
    }

    private static Medico MapearMedico(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        Nome = reader.GetString(1),
        Especialidade = reader.IsDBNull(2) ? "" : reader.GetString(2),
        HospitalId = null,
        FotoUrl = null,
        Rating = 5.0m, // Mock default rating
        DisponivelHoje = true // Mock
    };
}
