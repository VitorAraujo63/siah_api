using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseTriagemRepository : ITriagemRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseTriagemRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Guid?> ObterIdUsuarioPorCpfAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id FROM usuarios WHERE cpf = @cpf LIMIT 1";
        cmd.Parameters.AddWithValue("cpf", cpf);
        var resultado = await cmd.ExecuteScalarAsync();
        return resultado is Guid id ? id : null;
    }

    public async Task<Triagem> CriarAsync(Triagem triagem)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO triagens (id_usuario, queixa_principal, peso, altura, temperatura, pressao_arterial, frequencia_cardiaca)
            VALUES (@id_usuario, @queixa_principal, @peso::numeric, @altura::numeric, @temperatura, @pressao_arterial, @frequencia_cardiaca::int)
            RETURNING id, id_usuario, queixa_principal, peso::text, altura::text, temperatura, pressao_arterial, frequencia_cardiaca::text";

        cmd.Parameters.AddWithValue("id_usuario", triagem.IdUsuario);
        cmd.Parameters.AddWithValue("queixa_principal", triagem.QueixaPrincipal);
        cmd.Parameters.AddWithValue("peso", (object?)triagem.Peso ?? DBNull.Value);
        cmd.Parameters.AddWithValue("altura", (object?)triagem.Altura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("temperatura", (object?)triagem.Temperatura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("pressao_arterial", (object?)triagem.PressaoArterial ?? DBNull.Value);
        cmd.Parameters.AddWithValue("frequencia_cardiaca", (object?)triagem.FrequenciaCardiaca ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new Triagem
        {
            Id = reader.GetGuid(0),
            IdUsuario = reader.GetGuid(1),
            QueixaPrincipal = reader.GetString(2),
            Peso = reader.IsDBNull(3) ? null : reader.GetString(3),
            Altura = reader.IsDBNull(4) ? null : reader.GetString(4),
            Temperatura = reader.IsDBNull(5) ? null : reader.GetString(5),
            PressaoArterial = reader.IsDBNull(6) ? null : reader.GetString(6),
            FrequenciaCardiaca = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }
}
