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
            INSERT INTO senhas_atendimento (id_paciente, totem_id, tipo_servico, departamento_id, numero_senha, posicao_na_fila, tempo_espera_minutos, status)
            VALUES (@id_paciente, @totem_id, @tipo_servico, @departamento_id, @numero_senha, @posicao_na_fila, @tempo_espera_minutos, @status)
            RETURNING id, id_paciente, totem_id, tipo_servico, departamento_id, numero_senha, posicao_na_fila, tempo_espera_minutos, status, emitida_em";

        cmd.Parameters.AddWithValue("id_paciente", senha.IdPaciente);
        cmd.Parameters.AddWithValue("totem_id", senha.TotemId);
        cmd.Parameters.AddWithValue("tipo_servico", senha.TipoServico);
        cmd.Parameters.AddWithValue("departamento_id", senha.DepartamentoId);
        cmd.Parameters.AddWithValue("numero_senha", senha.NumeroSenha);
        cmd.Parameters.AddWithValue("posicao_na_fila", senha.PosicaoNaFila);
        cmd.Parameters.AddWithValue("tempo_espera_minutos", senha.TempoEsperaMinutos);
        cmd.Parameters.AddWithValue("status", senha.Status);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new SenhaAtendimento
        {
            Id = reader.GetGuid(0),
            IdPaciente = reader.GetGuid(1),
            TotemId = reader.GetString(2),
            TipoServico = reader.GetString(3),
            DepartamentoId = reader.GetGuid(4),
            NumeroSenha = reader.GetString(5),
            PosicaoNaFila = reader.GetInt32(6),
            TempoEsperaMinutos = reader.GetInt32(7),
            Status = reader.GetString(8),
            EmitidaEm = reader.GetDateTime(9)
        };
    }

    public async Task<SenhaAtendimento?> ObterSenhaAtivaPorUsuarioAsync(Guid userId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_paciente, totem_id, tipo_servico, departamento_id, numero_senha,
                   posicao_na_fila, tempo_espera_minutos, status, emitida_em, chamada_em
            FROM senhas_atendimento
            WHERE id_paciente = @id_paciente AND status IN ('waiting', 'called', 'in_service')
            ORDER BY emitida_em DESC LIMIT 1";
        cmd.Parameters.AddWithValue("id_paciente", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearSenha(reader);
    }

    public async Task<SenhaAtendimento?> ObterPorIdAsync(Guid ticketId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_paciente, totem_id, tipo_servico, departamento_id, numero_senha,
                   posicao_na_fila, tempo_espera_minutos, status, emitida_em, chamada_em
            FROM senhas_atendimento WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", ticketId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearSenha(reader);
    }

    public async Task<SenhaAtendimento> AtualizarStatusAsync(Guid ticketId, string novoStatus)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE senhas_atendimento SET status = @status WHERE id = @id";
        cmd.Parameters.AddWithValue("status", novoStatus);
        cmd.Parameters.AddWithValue("id", ticketId);
        await cmd.ExecuteNonQueryAsync();

        return (await ObterPorIdAsync(ticketId))!;
    }

    public async Task<int> ObterProximaPosicaoAsync(Guid departamentoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT COALESCE(MAX(posicao_na_fila), 0) + 1
            FROM senhas_atendimento
            WHERE departamento_id = @departamento_id AND status IN ('waiting', 'called')";
        cmd.Parameters.AddWithValue("departamento_id", departamentoId);

        return (int)(long)(await cmd.ExecuteScalarAsync() ?? 1L);
    }

    private static SenhaAtendimento MapearSenha(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        IdPaciente = reader.GetGuid(1),
        TotemId = reader.GetString(2),
        TipoServico = reader.GetString(3),
        DepartamentoId = reader.GetGuid(4),
        NumeroSenha = reader.GetString(5),
        PosicaoNaFila = reader.GetInt32(6),
        TempoEsperaMinutos = reader.GetInt32(7),
        Status = reader.GetString(8),
        EmitidaEm = reader.GetDateTime(9),
        ChamadaEm = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
    };
}
