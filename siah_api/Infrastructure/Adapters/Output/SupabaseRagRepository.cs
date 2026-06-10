using Npgsql;
using SiahApi.Application.DTOs.Rag;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseRagRepository : IRagRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseRagRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<RagPatientDto?> ObterPacientePorCpfAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT id, nome, cpf, data_nascimento
            FROM usuarios
            WHERE cpf = @cpf
            LIMIT 1";

        cmd.Parameters.AddWithValue("cpf", cpf);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new RagPatientDto
        {
            Id = reader.GetGuid(0).ToString(),
            Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
            Cpf = reader.GetString(2),
            BirthDate = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3).ToDateTime(TimeOnly.MinValue)
        };
    }

    public async Task<IEnumerable<RagConsultationDto>> ListarConsultasPorCpfAsync(string cpf)
    {
        var resultado = new List<RagConsultationDto>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT
                c.id,
                c.motivo_consulta,
                c.diagnostico,
                c.data_consulta,
                c.anotacoes_medicas,
                c.prescricao
            FROM consultas c
            INNER JOIN usuarios u ON c.id_usuario = u.id
            WHERE u.cpf = @cpf
            ORDER BY c.data_consulta DESC";

        cmd.Parameters.AddWithValue("cpf", cpf);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var prescricao = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            var medications = ParsearMedicamentos(prescricao);

            resultado.Add(new RagConsultationDto
            {
                Id = reader.GetGuid(0).ToString(),
                Reason = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                FinalDiagnosis = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Date = reader.GetDateTime(3),
                Observations = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Medications = medications
            });
        }

        return resultado;
    }

    private static List<RagMedicationDto> ParsearMedicamentos(string prescricao)
    {
        if (string.IsNullOrWhiteSpace(prescricao))
            return new List<RagMedicationDto>();

        return prescricao
            .Split(new[] { ',', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(m => new RagMedicationDto { Name = m.Trim() })
            .Where(m => !string.IsNullOrWhiteSpace(m.Name))
            .ToList();
    }
}
