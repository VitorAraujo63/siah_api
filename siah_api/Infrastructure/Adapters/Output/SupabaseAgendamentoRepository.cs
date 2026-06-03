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
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas
            WHERE id_usuario = @id_usuario
            ORDER BY data_consulta DESC
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
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas
            WHERE id_usuario = @id_usuario AND data_consulta >= NOW()
            ORDER BY data_consulta ASC";

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
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas WHERE id = @id LIMIT 1";

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
                SELECT 1 FROM consultas
                WHERE id_profissional = @medico_id AND data_consulta::date = @data::date AND TO_CHAR(data_consulta, 'HH24:MI') = @horario
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
            INSERT INTO consultas (id_usuario, id_profissional, id_hospital, data_consulta, motivo_consulta, diagnostico)
            VALUES (@id_usuario, @medico_id, (SELECT id FROM hospitais LIMIT 1), @data_consulta::timestamp, @motivo_consulta, '')
            RETURNING id, id_usuario, id_profissional, data_consulta, motivo_consulta";

        cmd.Parameters.AddWithValue("id_usuario", agendamento.IdUsuario);
        cmd.Parameters.AddWithValue("medico_id", agendamento.MedicoId);
        cmd.Parameters.AddWithValue("data_consulta", $"{agendamento.Data} {agendamento.Horario}");
        cmd.Parameters.AddWithValue("motivo_consulta", (object?)agendamento.Observacoes ?? "Consulta de rotina");

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return MapearAgendamento(reader);
    }

    public async Task<Agendamento> AtualizarAsync(Agendamento agendamento)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            UPDATE consultas SET
                data_consulta = @data_consulta::timestamp,
                motivo_consulta = @motivo_consulta
            WHERE id = @id";

        cmd.Parameters.AddWithValue("id", agendamento.Id);
        cmd.Parameters.AddWithValue("data_consulta", $"{agendamento.Data} {agendamento.Horario}");
        cmd.Parameters.AddWithValue("motivo_consulta", (object?)agendamento.Observacoes ?? "Consulta de rotina");

        await cmd.ExecuteNonQueryAsync();
        return agendamento;
    }

    private static Agendamento MapearAgendamento(NpgsqlDataReader reader)
    {
        var dataConsulta = reader.GetDateTime(3);
        return new Agendamento
        {
            Id = reader.GetGuid(0),
            IdUsuario = reader.IsDBNull(1) ? Guid.Empty : reader.GetGuid(1),
            MedicoId = reader.IsDBNull(2) ? Guid.Empty : reader.GetGuid(2),
            EspecialidadeId = Guid.Empty, // Mock
            Data = dataConsulta.ToString("yyyy-MM-dd"),
            Horario = dataConsulta.ToString("HH:mm"),
            Observacoes = reader.IsDBNull(4) ? null : reader.GetString(4),
            PlanoId = null,
            Status = dataConsulta > DateTime.UtcNow ? "scheduled" : "completed",
            NumeroDaSenha = null,
            MotivoCancelamento = null,
            TipoCancelamento = null,
            ReembolsoElegivel = false,
            CanceladoEm = null
        };
    }
}
