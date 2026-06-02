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
        cmd.CommandText = "SELECT id, nome, especialidade, hospital_id, foto_url, rating, disponivel_hoje FROM medicos WHERE 1=1";

        if (!string.IsNullOrEmpty(filtros.Name))
        {
            cmd.CommandText += " AND nome ILIKE @nome";
            cmd.Parameters.AddWithValue("nome", $"%{filtros.Name}%");
        }
        if (filtros.AvailableToday.HasValue)
        {
            cmd.CommandText += " AND disponivel_hoje = @disponivel_hoje";
            cmd.Parameters.AddWithValue("disponivel_hoje", filtros.AvailableToday.Value);
        }
        if (filtros.RatingMin.HasValue)
        {
            cmd.CommandText += " AND rating >= @rating_min";
            cmd.Parameters.AddWithValue("rating_min", filtros.RatingMin.Value);
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearMedico(reader));

        return resultado;
    }

    public async Task<IEnumerable<Medico>> ListarFavoritosPorUsuarioAsync(Guid userId)
    {
        var resultado = new List<Medico>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT m.id, m.nome, m.especialidade, m.hospital_id, m.foto_url, m.rating, m.disponivel_hoje
            FROM medicos m
            INNER JOIN medicos_favoritos mf ON m.id = mf.id_medico
            WHERE mf.id_usuario = @id_usuario";
        cmd.Parameters.AddWithValue("id_usuario", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearMedico(reader));

        return resultado;
    }

    public async Task<Medico?> ObterPorIdAsync(Guid medicoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, especialidade, hospital_id, foto_url, rating, disponivel_hoje FROM medicos WHERE id = @id LIMIT 1";
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

    public async Task<IEnumerable<(string Avaliador, string? Comentario, decimal Nota, DateTime Data)>> ObterAvaliacoesAsync(Guid medicoId)
    {
        var resultado = new List<(string, string?, decimal, DateTime)>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT avaliador, comentario, nota, criado_em FROM avaliacoes_medicos WHERE id_medico = @id_medico ORDER BY criado_em DESC";
        cmd.Parameters.AddWithValue("id_medico", medicoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add((
                reader.GetString(0),
                reader.IsDBNull(1) ? null : reader.GetString(1),
                reader.GetDecimal(2),
                reader.GetDateTime(3)
            ));
        }

        return resultado;
    }

    public async Task FavoritarAsync(Guid userId, Guid medicoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO medicos_favoritos (id_usuario, id_medico) VALUES (@id_usuario, @id_medico) ON CONFLICT DO NOTHING";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("id_medico", medicoId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DesfavoritarAsync(Guid userId, Guid medicoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM medicos_favoritos WHERE id_usuario = @id_usuario AND id_medico = @id_medico";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("id_medico", medicoId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Medico>> BuscarAsync(string termo)
    {
        var resultado = new List<Medico>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, nome, especialidade, hospital_id, foto_url, rating, disponivel_hoje
            FROM medicos
            WHERE nome ILIKE @termo OR especialidade ILIKE @termo";
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
        Especialidade = reader.GetString(2),
        HospitalId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
        FotoUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
        Rating = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
        DisponivelHoje = !reader.IsDBNull(6) && reader.GetBoolean(6)
    };
}
