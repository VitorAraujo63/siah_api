using Npgsql;
using NpgsqlTypes;
using SiahApi.Application.DTOs.Exame;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseExameRepository : IExameRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseExameRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<ExameResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        var resultado = new List<ExameResponse>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        var filtro = string.IsNullOrWhiteSpace(pesquisa)
            ? string.Empty
            : "AND e.tipo_exame ILIKE @pesquisa";

        cmd.CommandText = $@"
            SELECT e.id, e.tipo_exame, e.data_realizacao::text, e.resultado_link, e.nome_laboratorio
            FROM exames e
            INNER JOIN usuarios u ON u.id = e.id_usuario
            WHERE u.cpf = @cpf
            {filtro}
            ORDER BY e.data_realizacao DESC";

        cmd.Parameters.AddWithValue("cpf", cpf);

        if (!string.IsNullOrWhiteSpace(pesquisa))
            cmd.Parameters.AddWithValue("pesquisa", $"%{pesquisa}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new ExameResponse
            {
                Id = reader.GetGuid(0),
                Exam = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                DateExam = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                PdfUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                NameLaboratory = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return resultado;
    }

    public async Task<ExameResponse> CriarAsync(CriarExameRequest request)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            INSERT INTO exames (id_usuario, tipo_exame, data_realizacao, resultado_link, nome_laboratorio)
            VALUES (@id_usuario, @tipo_exame, @data_realizacao, @resultado_link, @nome_laboratorio)
            RETURNING id, tipo_exame, data_realizacao::text, resultado_link, nome_laboratorio";

        cmd.Parameters.AddWithValue("id_usuario", request.IdPacient);
        cmd.Parameters.AddWithValue("tipo_exame", request.Exam);
        cmd.Parameters.Add(new NpgsqlParameter("data_realizacao", NpgsqlDbType.Date)
        {
            Value = !string.IsNullOrEmpty(request.DateExam)
                ? DateOnly.Parse(request.DateExam)
                : DBNull.Value
        });
        cmd.Parameters.AddWithValue("resultado_link", (object?)request.PdfUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("nome_laboratorio", (object?)request.NameLaboratory ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new ExameResponse
        {
            Id = reader.GetGuid(0),
            Exam = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
            DateExam = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            PdfUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
            NameLaboratory = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }
}
