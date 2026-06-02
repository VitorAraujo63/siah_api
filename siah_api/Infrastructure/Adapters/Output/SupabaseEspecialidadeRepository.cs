using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseEspecialidadeRepository : IEspecialidadeRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseEspecialidadeRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Especialidade>> ListarAsync()
    {
        var resultado = new List<Especialidade>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, descricao FROM especialidades ORDER BY nome";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Especialidade
            {
                Id = reader.GetGuid(0),
                Nome = reader.GetString(1),
                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<Medico>> ListarMedicosPorEspecialidadeAsync(Guid especialidadeId)
    {
        var resultado = new List<Medico>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, nome, especialidade, hospital_id, foto_url, rating, disponivel_hoje
            FROM medicos WHERE especialidade = (SELECT nome FROM especialidades WHERE id = @id)";
        cmd.Parameters.AddWithValue("id", especialidadeId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Medico
            {
                Id = reader.GetGuid(0),
                Nome = reader.GetString(1),
                Especialidade = reader.GetString(2),
                HospitalId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                FotoUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                Rating = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                DisponivelHoje = !reader.IsDBNull(6) && reader.GetBoolean(6)
            });
        }

        return resultado;
    }
}
