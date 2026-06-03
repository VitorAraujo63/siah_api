using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseFilaRepository : IFilaRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseFilaRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<SenhaAtendimento> EmitirSenhaAsync(SenhaAtendimento senha)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO senhas (cpf, numero_senha, especialidade, local_chamada, status)
            VALUES (@cpf, @numero_senha, @especialidade, @local_chamada, @status)
            RETURNING id, cpf, numero_senha, especialidade, local_chamada, created_at, status";

        cmd.Parameters.AddWithValue("cpf", senha.Cpf);
        cmd.Parameters.AddWithValue("numero_senha", senha.NumeroSenha);
        cmd.Parameters.AddWithValue("especialidade", senha.Especialidade);
        cmd.Parameters.AddWithValue("local_chamada", (object?)senha.LocalChamada ?? DBNull.Value);
        cmd.Parameters.AddWithValue("status", (object?)senha.Status ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return MapearSenha(reader);
    }

    public async Task<SenhaAtendimento?> ObterSenhaAtivaPorUsuarioAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, cpf, numero_senha, especialidade, local_chamada, created_at, status
            FROM senhas
            WHERE cpf = @cpf AND status IN ('waiting', 'called', 'in_service')
            ORDER BY created_at DESC LIMIT 1";
        cmd.Parameters.AddWithValue("cpf", cpf);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearSenha(reader);
    }

    public async Task<SenhaAtendimento?> ObterPorIdAsync(long ticketId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, cpf, numero_senha, especialidade, local_chamada, created_at, status
            FROM senhas WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", ticketId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearSenha(reader);
    }

    public async Task<SenhaAtendimento> AtualizarStatusAsync(long ticketId, string novoStatus)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE senhas SET status = @status WHERE id = @id";
        cmd.Parameters.AddWithValue("status", novoStatus);
        cmd.Parameters.AddWithValue("id", ticketId);
        await cmd.ExecuteNonQueryAsync();

        return (await ObterPorIdAsync(ticketId))!;
    }

    public async Task<int> ObterProximaPosicaoAsync(string especialidade)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT COUNT(*) + 1
            FROM senhas
            WHERE especialidade = @especialidade AND status IN ('waiting', 'called')";
        cmd.Parameters.AddWithValue("especialidade", especialidade);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync() ?? 1);
    }

    private static SenhaAtendimento MapearSenha(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt64(0),
        Cpf = reader.GetString(1),
        NumeroSenha = reader.GetString(2),
        Especialidade = reader.GetString(3),
        LocalChamada = reader.IsDBNull(4) ? null : reader.GetString(4),
        CreatedAt = reader.GetDateTime(5),
        Status = reader.IsDBNull(6) ? null : reader.GetString(6)
    };
}
