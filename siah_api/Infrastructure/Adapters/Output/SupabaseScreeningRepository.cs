using Npgsql;
using SiahApi.Application.DTOs.Screening;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseScreeningRepository : IScreeningRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseScreeningRepository(NpgsqlDataSource dataSource)
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

    public async Task<IEnumerable<ScreeningResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        var resultado = new List<ScreeningResponse>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        var filtro = string.IsNullOrWhiteSpace(pesquisa)
            ? string.Empty
            : "AND t.queixa_principal ILIKE @pesquisa";

        cmd.CommandText = $@"
            SELECT t.id, t.pressao_arterial, t.temperatura, t.frequencia_cardiaca,
                   t.peso, t.altura, t.queixa_principal, t.data_hora_triagem
            FROM triagens t
            INNER JOIN usuarios u ON t.id_usuario = u.id
            WHERE u.cpf = @cpf
            {filtro}
            ORDER BY t.data_hora_triagem DESC";

        cmd.Parameters.AddWithValue("cpf", cpf);

        if (!string.IsNullOrWhiteSpace(pesquisa))
            cmd.Parameters.AddWithValue("pesquisa", $"%{pesquisa}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new ScreeningResponse
            {
                Id = reader.GetGuid(0),
                BloodPressure = reader.IsDBNull(1) ? null : reader.GetString(1),
                Temperature = reader.IsDBNull(2) ? null : reader.GetString(2),
                HeartRate = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                Weight = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                Height = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                Complaint = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                DateScreening = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
        }

        return resultado;
    }

    public async Task<ScreeningResponse> CriarAsync(Guid idUsuario, CriarScreeningRequest request)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            INSERT INTO triagens (id_usuario, pressao_arterial, temperatura, frequencia_cardiaca, peso, altura, queixa_principal, data_hora_triagem)
            VALUES (@id_usuario, @pressao_arterial, @temperatura, @frequencia_cardiaca, @peso, @altura, @queixa_principal, @data_hora_triagem)
            RETURNING id, pressao_arterial, temperatura, frequencia_cardiaca, peso, altura, queixa_principal, data_hora_triagem";

        cmd.Parameters.AddWithValue("id_usuario", idUsuario);
        cmd.Parameters.AddWithValue("pressao_arterial", (object?)request.BloodPressure ?? DBNull.Value);
        cmd.Parameters.AddWithValue("temperatura", (object?)request.Temperature ?? DBNull.Value);
        cmd.Parameters.AddWithValue("frequencia_cardiaca", (object?)request.HeartRate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("peso", (object?)request.Weight ?? DBNull.Value);
        cmd.Parameters.AddWithValue("altura", (object?)request.Height ?? DBNull.Value);
        cmd.Parameters.AddWithValue("queixa_principal", request.Complaint);
        cmd.Parameters.AddWithValue("data_hora_triagem", (object?)request.DateScreening ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new ScreeningResponse
        {
            Id = reader.GetGuid(0),
            BloodPressure = reader.IsDBNull(1) ? null : reader.GetString(1),
            Temperature = reader.IsDBNull(2) ? null : reader.GetString(2),
            HeartRate = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            Weight = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
            Height = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
            Complaint = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
            DateScreening = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        };
    }
}
