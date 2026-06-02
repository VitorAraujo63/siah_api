using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabasePacienteRepository : IPacienteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabasePacienteRepository(NpgsqlDataSource dataSource)
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

    public async Task<Paciente> CriarAsync(Paciente paciente)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO usuarios (
                nome, cpf, email, telefone, data_nascimento, genero, tipo_sanguineo,
                hospital_vinculado, rg, cartao_sus, cnh, cep, rua, numero, bairro,
                cidade, estado, possui_plano_saude, nome_plano, numero_carteirinha,
                validade_carteirinha, nome_responsavel, parentesco, telefone_responsavel,
                images, embedding, embedding_path
            ) VALUES (
                @nome, @cpf, @email, @telefone, @data_nascimento, @genero, @tipo_sanguineo,
                @hospital_vinculado, @rg, @cartao_sus, @cnh, @cep, @rua, @numero, @bairro,
                @cidade, @estado, @possui_plano_saude, @nome_plano, @numero_carteirinha,
                @validade_carteirinha, @nome_responsavel, @parentesco, @telefone_responsavel,
                @images, @embedding, @embedding_path
            )
            RETURNING id, nome, cpf, embedding_path, images";

        cmd.Parameters.AddWithValue("nome", paciente.Nome);
        cmd.Parameters.AddWithValue("cpf", paciente.Cpf);
        cmd.Parameters.AddWithValue("email", (object?)paciente.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("telefone", (object?)paciente.Telefone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("data_nascimento", (object?)paciente.DataNascimento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("genero", (object?)paciente.Genero ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tipo_sanguineo", (object?)paciente.TipoSanguineo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("hospital_vinculado", (object?)paciente.HospitalVinculado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rg", (object?)paciente.Rg ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cartao_sus", (object?)paciente.CartaoSus ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cnh", (object?)paciente.Cnh ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cep", (object?)paciente.Cep ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rua", (object?)paciente.Rua ?? DBNull.Value);
        cmd.Parameters.AddWithValue("numero", (object?)paciente.Numero ?? DBNull.Value);
        cmd.Parameters.AddWithValue("bairro", (object?)paciente.Bairro ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cidade", (object?)paciente.Cidade ?? DBNull.Value);
        cmd.Parameters.AddWithValue("estado", (object?)paciente.Estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("possui_plano_saude", paciente.PossuiPlanoSaude);
        cmd.Parameters.AddWithValue("nome_plano", (object?)paciente.NomePlano ?? DBNull.Value);
        cmd.Parameters.AddWithValue("numero_carteirinha", (object?)paciente.NumeroCarteirinha ?? DBNull.Value);
        cmd.Parameters.AddWithValue("validade_carteirinha", (object?)paciente.ValidadeCarteirinha ?? DBNull.Value);
        cmd.Parameters.AddWithValue("nome_responsavel", (object?)paciente.NomeResponsavel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("parentesco", (object?)paciente.Parentesco ?? DBNull.Value);
        cmd.Parameters.AddWithValue("telefone_responsavel", (object?)paciente.TelefoneResponsavel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("images", paciente.Images.ToArray());
        cmd.Parameters.AddWithValue("embedding", (object?)paciente.Embedding ?? DBNull.Value);
        cmd.Parameters.AddWithValue("embedding_path", (object?)paciente.EmbeddingPath ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new Paciente
        {
            Id = reader.GetGuid(0),
            Nome = reader.GetString(1),
            Cpf = reader.GetString(2),
            EmbeddingPath = reader.IsDBNull(3) ? null : reader.GetString(3),
            Images = reader.IsDBNull(4) ? new List<string>() : ((string[])reader[4]).ToList()
        };
    }

    public async Task<IEnumerable<(Guid Id, string Nome, string Cpf, float[] Embedding)>> ListarComEmbeddingAsync()
    {
        var resultado = new List<(Guid, string, string, float[])>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, cpf, embedding FROM usuarios WHERE embedding IS NOT NULL";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add((
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                (float[])reader[3]
            ));
        }

        return resultado;
    }
}
