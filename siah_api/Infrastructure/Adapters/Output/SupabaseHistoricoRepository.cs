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
            SELECT id, id_usuario, medico_id, especialidade_id, data, horario, status
            FROM agendamentos
            WHERE id_usuario = @id_usuario AND status IN ('completed', 'cancelled', 'no_show')
            ORDER BY data DESC LIMIT @per_page OFFSET @offset";

        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("per_page", filtros.PerPage);
        cmd.Parameters.AddWithValue("offset", (filtros.Page - 1) * filtros.PerPage);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Agendamento
            {
                Id = reader.GetGuid(0),
                IdUsuario = reader.GetGuid(1),
                MedicoId = reader.GetGuid(2),
                EspecialidadeId = reader.GetGuid(3),
                Data = reader.GetString(4),
                Horario = reader.GetString(5),
                Status = reader.GetString(6)
            });
        }

        return resultado;
    }

    public async Task<Agendamento?> ObterHistoricoPorIdAsync(Guid atendimentoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, id_usuario, medico_id, especialidade_id, data, horario, status FROM agendamentos WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", atendimentoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Agendamento
        {
            Id = reader.GetGuid(0),
            IdUsuario = reader.GetGuid(1),
            MedicoId = reader.GetGuid(2),
            EspecialidadeId = reader.GetGuid(3),
            Data = reader.GetString(4),
            Horario = reader.GetString(5),
            Status = reader.GetString(6)
        };
    }

    public async Task<IEnumerable<Agendamento>> ListarRecentesPorUsuarioAsync(Guid userId)
    {
        var resultado = new List<Agendamento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, medico_id, especialidade_id, data, horario, status
            FROM agendamentos WHERE id_usuario = @id_usuario
            ORDER BY data DESC LIMIT 10";
        cmd.Parameters.AddWithValue("id_usuario", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Agendamento
            {
                Id = reader.GetGuid(0),
                IdUsuario = reader.GetGuid(1),
                MedicoId = reader.GetGuid(2),
                EspecialidadeId = reader.GetGuid(3),
                Data = reader.GetString(4),
                Horario = reader.GetString(5),
                Status = reader.GetString(6)
            });
        }

        return resultado;
    }
}
