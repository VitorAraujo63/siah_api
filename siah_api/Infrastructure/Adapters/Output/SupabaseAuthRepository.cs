using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseAuthRepository : IAuthRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseAuthRepository(NpgsqlDataSource dataSource)
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
            INSERT INTO usuarios (nome, cpf, email, telefone, data_nascimento, senha)
            VALUES (@nome, @cpf, @email, @telefone, @data_nascimento::date, @senha)
            RETURNING id, nome, cpf";

        cmd.Parameters.AddWithValue("nome", paciente.Nome);
        cmd.Parameters.AddWithValue("cpf", paciente.Cpf);
        cmd.Parameters.AddWithValue("email", (object?)paciente.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("telefone", (object?)paciente.Telefone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("data_nascimento", (object?)paciente.DataNascimento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("senha", (object?)paciente.SenhaHash ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new Paciente { Id = reader.GetGuid(0), Nome = reader.GetString(1), Cpf = reader.GetString(2) };
    }

    public async Task<Paciente?> ObterPorCpfAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome, cpf, senha FROM usuarios WHERE cpf = @cpf LIMIT 1";
        cmd.Parameters.AddWithValue("cpf", cpf);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Paciente
        {
            Id = reader.GetGuid(0),
            Nome = reader.GetString(1),
            Cpf = reader.GetString(2),
            SenhaHash = reader.IsDBNull(3) ? null : reader.GetString(3)
        };
    }

    public async Task<Paciente?> ObterPorIdAsync(Guid id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, nome, cpf, email, telefone, data_nascimento::text, genero, tipo_sanguineo,
                   hospital_vinculado, cep, rua, numero, bairro, cidade, estado,
                   possui_plano_saude, nome_plano, numero_carteirinha, validade_carteirinha::text,
                   nome_responsavel, parentesco, telefone_responsavel
            FROM usuarios WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Paciente
        {
            Id = reader.GetGuid(0),
            Nome = reader.GetString(1),
            Cpf = reader.GetString(2),
            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
            Telefone = reader.IsDBNull(4) ? null : reader.GetString(4),
            DataNascimento = reader.IsDBNull(5) ? null : reader.GetString(5),
            Genero = reader.IsDBNull(6) ? null : reader.GetString(6),
            TipoSanguineo = reader.IsDBNull(7) ? null : reader.GetString(7),
            HospitalVinculado = reader.IsDBNull(8) ? null : reader.GetString(8),
            Cep = reader.IsDBNull(9) ? null : reader.GetString(9),
            Rua = reader.IsDBNull(10) ? null : reader.GetString(10),
            Numero = reader.IsDBNull(11) ? null : reader.GetString(11),
            Bairro = reader.IsDBNull(12) ? null : reader.GetString(12),
            Cidade = reader.IsDBNull(13) ? null : reader.GetString(13),
            Estado = reader.IsDBNull(14) ? null : reader.GetString(14),
            PossuiPlanoSaude = reader.IsDBNull(15) ? false : reader.GetBoolean(15),
            NomePlano = reader.IsDBNull(16) ? null : reader.GetString(16),
            NumeroCarteirinha = reader.IsDBNull(17) ? null : reader.GetString(17),
            ValidadeCarteirinha = reader.IsDBNull(18) ? null : reader.GetString(18),
            NomeResponsavel = reader.IsDBNull(19) ? null : reader.GetString(19),
            Parentesco = reader.IsDBNull(20) ? null : reader.GetString(20),
            TelefoneResponsavel = reader.IsDBNull(21) ? null : reader.GetString(21)
        };
    }

    public async Task<Paciente> AtualizarAsync(Paciente paciente)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE usuarios SET
                nome = @nome, email = @email, telefone = @telefone, data_nascimento = @data_nascimento::date,
                genero = @genero, cep = @cep, rua = @rua, numero = @numero, bairro = @bairro,
                cidade = @cidade, estado = @estado
            WHERE id = @id";

        cmd.Parameters.AddWithValue("id", paciente.Id);
        cmd.Parameters.AddWithValue("nome", paciente.Nome);
        cmd.Parameters.AddWithValue("email", (object?)paciente.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("telefone", (object?)paciente.Telefone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("data_nascimento", (object?)paciente.DataNascimento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("genero", (object?)paciente.Genero ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cep", (object?)paciente.Cep ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rua", (object?)paciente.Rua ?? DBNull.Value);
        cmd.Parameters.AddWithValue("numero", (object?)paciente.Numero ?? DBNull.Value);
        cmd.Parameters.AddWithValue("bairro", (object?)paciente.Bairro ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cidade", (object?)paciente.Cidade ?? DBNull.Value);
        cmd.Parameters.AddWithValue("estado", (object?)paciente.Estado ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
        return paciente;
    }

    public async Task InvalidarTokenAsync(Guid userId, string token)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO tokens_invalidados (id_usuario, token) VALUES (@id_usuario, @token)";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("token", token);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> TokenEstaInvalidadoAsync(string token)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM tokens_invalidados WHERE token = @token)";
        cmd.Parameters.AddWithValue("token", token);
        return (bool)(await cmd.ExecuteScalarAsync() ?? false);
    }

    public async Task SalvarRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiracao)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO refresh_tokens (id_usuario, token, expira_em) VALUES (@id_usuario, @token, @expira_em)";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("token", refreshToken);
        cmd.Parameters.AddWithValue("expira_em", expiracao);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<(Guid UserId, bool Valido)> ValidarRefreshTokenAsync(string refreshToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id_usuario, expira_em FROM refresh_tokens WHERE token = @token LIMIT 1";
        cmd.Parameters.AddWithValue("token", refreshToken);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return (Guid.Empty, false);

        var userId = reader.GetGuid(0);
        var expiracao = reader.GetDateTime(1);

        return (userId, expiracao > DateTime.UtcNow);
    }

    public async Task RevogarRefreshTokenAsync(string refreshToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM refresh_tokens WHERE token = @token";
        cmd.Parameters.AddWithValue("token", refreshToken);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Paciente> CriarOuAtualizarTotemAsync(Paciente paciente)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO usuarios (nome, cpf, email)
            VALUES (@nome, @cpf, @email)
            ON CONFLICT (cpf) DO UPDATE SET nome = EXCLUDED.nome
            RETURNING id, nome, cpf";

        cmd.Parameters.AddWithValue("nome", paciente.Nome);
        cmd.Parameters.AddWithValue("cpf", paciente.Cpf);
        cmd.Parameters.AddWithValue("email", (object?)paciente.Email ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new Paciente { Id = reader.GetGuid(0), Nome = reader.GetString(1), Cpf = reader.GetString(2) };
    }
}
