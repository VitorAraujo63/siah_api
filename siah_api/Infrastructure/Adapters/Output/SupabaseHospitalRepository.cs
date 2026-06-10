using Npgsql;
using SiahApi.Application.DTOs.Hospital;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseHospitalRepository : IHospitalRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseHospitalRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<HospitalResponse>> ListarAsync()
    {
        var resultado = new List<HospitalResponse>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, nome_hospital, cep, rua, numero, complemento, bairro, cidade, estado
            FROM hospitais
            ORDER BY nome_hospital ASC";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new HospitalResponse
            {
                Id = reader.GetGuid(0),
                NomeHospital = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Cep = reader.IsDBNull(2) ? null : reader.GetString(2),
                Rua = reader.IsDBNull(3) ? null : reader.GetString(3),
                Numero = reader.IsDBNull(4) ? null : reader.GetString(4),
                Complemento = reader.IsDBNull(5) ? null : reader.GetString(5),
                Bairro = reader.IsDBNull(6) ? null : reader.GetString(6),
                Cidade = reader.IsDBNull(7) ? null : reader.GetString(7),
                Estado = reader.IsDBNull(8) ? null : reader.GetString(8)
            });
        }

        return resultado;
    }
}
