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
        cmd.CommandText = "SELECT DISTINCT especialidade FROM profissionais WHERE especialidade IS NOT NULL ORDER BY especialidade";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new Especialidade
            {
                Id = Guid.NewGuid(), // Mock
                Nome = reader.GetString(0),
                Descricao = null
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<Medico>> ListarMedicosPorEspecialidadeAsync(Guid especialidadeId)
    {
        // Como não temos tabela de especialidade, teremos que retornar vazio ou ignorar o filtro
        return new List<Medico>();
    }
}
