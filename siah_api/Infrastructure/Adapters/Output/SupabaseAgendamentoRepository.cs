using Npgsql;
using SiahApi.Application.DTOs.Agendamento;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseAgendamentoRepository : IAgendamentoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseAgendamentoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Agendamento>> ListarPorUsuarioAsync(Guid userId, AgendamentoFiltros filtros)
    {
        var resultado = new List<Agendamento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, id_usuario, medico_id, especialidade_id, data, horario, observacoes,
                   plano_id, status, numero_da_senha, motivo_cancelamento, tipo_cancelamento,
                   reembolso_elegivel, cancelado_em
            FROM agendamentos
            WHERE id_usuario = @id_usuario
            ORDER BY data DESC, horario DESC
            LIMIT @per_page OFFSET @offset";

        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("per_page", filtros.PerPage);
        cmd.Parameters.AddWithValue("offset", (filtros.Page - 1) * filtros.PerPage);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearAgendamento(reader));

        return resultado;
    }

    public async Task<IEnumerable<Agendamento>> ListarProximosPorUsuarioAsync(Guid userId)
    {
        var resultado = new List<Agendamento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, id_usuario, medico_id, especialidade_id, data, horario, observacoes,
                   plano_id, status, numero_da_senha, motivo_cancelamento, tipo_cancelamento,
                   reembolso_elegivel, cancelado_em
            FROM agendamentos
            WHERE id_usuario = @id_usuario AND status = 'scheduled'
            ORDER BY data ASC, horario ASC";

        cmd.Parameters.AddWithValue("id_usuario", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearAgendamento(reader));

        return resultado;
    }

    public async Task<Agendamento?> ObterPorIdAsync(Guid agendamentoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, id_usuario, medico_id, especialidade_id, data, horario, observacoes,
                   plano_id, status, numero_da_senha, motivo_cancelamento, tipo_cancelamento,
                   reembolso_elegivel, cancelado_em
            FROM agendamentos WHERE id = @id LIMIT 1";

        cmd.Parameters.AddWithValue("id", agendamentoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearAgendamento(reader);
    }

    public async Task<bool> HorarioDisponivelAsync(Guid medicoId, string data, string horario)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT NOT EXISTS(
                SELECT 1 FROM agendamentos
                WHERE medico_id = @medico_id AND data = @data AND horario = @horario AND status = 'scheduled'
            )";

        cmd.Parameters.AddWithValue("medico_id", medicoId);
        cmd.Parameters.AddWithValue("data", data);
        cmd.Parameters.AddWithValue("horario", horario);

        return (bool)(await cmd.ExecuteScalarAsync() ?? true);
    }

    public async Task<Agendamento> CriarAsync(Agendamento agendamento)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            INSERT INTO agendamentos (id_usuario, medico_id, especialidade_id, data, horario, observacoes, plano_id, status)
            VALUES (@id_usuario, @medico_id, @especialidade_id, @data, @horario, @observacoes, @plano_id, @status)
            RETURNING id, id_usuario, medico_id, especialidade_id, data, horario, observacoes,
                      plano_id, status, numero_da_senha, motivo_cancelamento, tipo_cancelamento,
                      reembolso_elegivel, cancelado_em";

        cmd.Parameters.AddWithValue("id_usuario", agendamento.IdUsuario);
        cmd.Parameters.AddWithValue("medico_id", agendamento.MedicoId);
        cmd.Parameters.AddWithValue("especialidade_id", agendamento.EspecialidadeId);
        cmd.Parameters.AddWithValue("data", agendamento.Data);
        cmd.Parameters.AddWithValue("horario", agendamento.Horario);
        cmd.Parameters.AddWithValue("observacoes", (object?)agendamento.Observacoes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("plano_id", (object?)agendamento.PlanoId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("status", agendamento.Status);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return MapearAgendamento(reader);
    }

    public async Task<Agendamento> AtualizarAsync(Agendamento agendamento)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            UPDATE agendamentos SET
                data = @data, horario = @horario, status = @status,
                motivo_cancelamento = @motivo_cancelamento, tipo_cancelamento = @tipo_cancelamento,
                reembolso_elegivel = @reembolso_elegivel, cancelado_em = @cancelado_em
            WHERE id = @id";

        cmd.Parameters.AddWithValue("id", agendamento.Id);
        cmd.Parameters.AddWithValue("data", agendamento.Data);
        cmd.Parameters.AddWithValue("horario", agendamento.Horario);
        cmd.Parameters.AddWithValue("status", agendamento.Status);
        cmd.Parameters.AddWithValue("motivo_cancelamento", (object?)agendamento.MotivoCancelamento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tipo_cancelamento", (object?)agendamento.TipoCancelamento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("reembolso_elegivel", agendamento.ReembolsoElegivel);
        cmd.Parameters.AddWithValue("cancelado_em", (object?)agendamento.CanceladoEm ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
        return agendamento;
    }

    private static Agendamento MapearAgendamento(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        IdUsuario = reader.GetGuid(1),
        MedicoId = reader.GetGuid(2),
        EspecialidadeId = reader.GetGuid(3),
        Data = reader.GetString(4),
        Horario = reader.GetString(5),
        Observacoes = reader.IsDBNull(6) ? null : reader.GetString(6),
        PlanoId = reader.IsDBNull(7) ? null : reader.GetGuid(7),
        Status = reader.GetString(8),
        NumeroDaSenha = reader.IsDBNull(9) ? null : reader.GetInt32(9),
        MotivoCancelamento = reader.IsDBNull(10) ? null : reader.GetString(10),
        TipoCancelamento = reader.IsDBNull(11) ? null : reader.GetString(11),
        ReembolsoElegivel = !reader.IsDBNull(12) && reader.GetBoolean(12),
        CanceladoEm = reader.IsDBNull(13) ? null : reader.GetDateTime(13)
    };
}
