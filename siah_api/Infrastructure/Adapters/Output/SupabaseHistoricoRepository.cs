using Npgsql;
using SiahApi.Application.DTOs.Historico;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseHistoricoRepository : IHistoricoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseHistoricoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Agendamento>> ListarHistoricoPorUsuarioAsync(Guid userId, HistoricoFiltros filtros)
    {
        var resultado = new List<Agendamento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas
            WHERE id_usuario = @id_usuario AND data_consulta < NOW()
            ORDER BY data_consulta DESC LIMIT @per_page OFFSET @offset";

        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("per_page", filtros.PerPage);
        cmd.Parameters.AddWithValue("offset", (filtros.Page - 1) * filtros.PerPage);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var dataConsulta = reader.GetDateTime(3);
            resultado.Add(new Agendamento
            {
                Id = reader.GetGuid(0),
                IdUsuario = reader.IsDBNull(1) ? Guid.Empty : reader.GetGuid(1),
                MedicoId = reader.IsDBNull(2) ? Guid.Empty : reader.GetGuid(2),
                EspecialidadeId = Guid.Empty,
                Data = dataConsulta.ToString("yyyy-MM-dd"),
                Horario = dataConsulta.ToString("HH:mm"),
                Status = "completed",
                Observacoes = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return resultado;
    }

    public async Task<Agendamento?> ObterHistoricoPorIdAsync(Guid atendimentoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", atendimentoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var dataConsulta = reader.GetDateTime(3);
        return new Agendamento
        {
            Id = reader.GetGuid(0),
            IdUsuario = reader.IsDBNull(1) ? Guid.Empty : reader.GetGuid(1),
            MedicoId = reader.IsDBNull(2) ? Guid.Empty : reader.GetGuid(2),
            EspecialidadeId = Guid.Empty,
            Data = dataConsulta.ToString("yyyy-MM-dd"),
            Horario = dataConsulta.ToString("HH:mm"),
            Status = "completed",
            Observacoes = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }

    public async Task<IEnumerable<Agendamento>> ListarRecentesPorUsuarioAsync(Guid userId)
    {
        var resultado = new List<Agendamento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, id_profissional, data_consulta, motivo_consulta
            FROM consultas WHERE id_usuario = @id_usuario AND data_consulta < NOW()
            ORDER BY data_consulta DESC LIMIT 10";
        cmd.Parameters.AddWithValue("id_usuario", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var dataConsulta = reader.GetDateTime(3);
            resultado.Add(new Agendamento
            {
                Id = reader.GetGuid(0),
                IdUsuario = reader.IsDBNull(1) ? Guid.Empty : reader.GetGuid(1),
                MedicoId = reader.IsDBNull(2) ? Guid.Empty : reader.GetGuid(2),
                EspecialidadeId = Guid.Empty,
                Data = dataConsulta.ToString("yyyy-MM-dd"),
                Horario = dataConsulta.ToString("HH:mm"),
                Status = "completed",
                Observacoes = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return resultado;
    }
}
