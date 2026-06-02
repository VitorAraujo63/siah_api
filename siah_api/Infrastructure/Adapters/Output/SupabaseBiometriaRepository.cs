using Npgsql;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseBiometriaRepository : IBiometriaRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseBiometriaRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<bool> ExistePorCpfAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM usuarios WHERE cpf = @cpf)";
        cmd.Parameters.AddWithValue("cpf", cpf);
        return (bool)(await cmd.ExecuteScalarAsync() ?? false);
    }

    public async Task<bool> AtualizarTemplateBiometricoAsync(string cpf, byte[] template)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE usuarios SET template_biometrico = @template WHERE cpf = @cpf";
        cmd.Parameters.AddWithValue("template", template);
        cmd.Parameters.AddWithValue("cpf", cpf);
        var linhasAfetadas = await cmd.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }

    public async Task<IEnumerable<(string Cpf, string Nome, byte[] Template)>> ListarComTemplateBiometricoAsync()
    {
        var resultado = new List<(string, string, byte[])>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT cpf, nome, template_biometrico FROM usuarios WHERE template_biometrico IS NOT NULL";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add((
                reader.GetString(0),
                reader.GetString(1),
                (byte[])reader[2]
            ));
        }

        return resultado;
    }
}
