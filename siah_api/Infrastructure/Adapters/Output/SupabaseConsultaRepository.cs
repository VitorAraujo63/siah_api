using Npgsql;
using SiahApi.Application.DTOs.Consulta;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseConsultaRepository : IConsultaRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseConsultaRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<ConsultaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        var resultado = new List<ConsultaResponse>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        var filtro = string.IsNullOrWhiteSpace(pesquisa)
            ? string.Empty
            : "AND c.motivo_consulta ILIKE @pesquisa";

        cmd.CommandText = $@"
            SELECT
                c.id,
                c.motivo_consulta,
                c.diagnostico,
                c.data_consulta,
                c.anotacoes_medicas,
                c.prescricao,
                p.nome,
                p.email,
                p.especialidade,
                p.tipo_profissional,
                h.nome_hospital,
                h.cep,
                h.rua,
                h.numero,
                h.complemento,
                h.bairro,
                h.cidade,
                h.estado
            FROM consultas c
            INNER JOIN usuarios u ON c.id_usuario = u.id
            LEFT JOIN profissionais p ON c.id_profissional = p.id
            LEFT JOIN hospitais h ON c.id_hospital = h.id
            WHERE u.cpf = @cpf
            {filtro}
            ORDER BY c.data_consulta DESC";

        cmd.Parameters.AddWithValue("cpf", cpf);

        if (!string.IsNullOrWhiteSpace(pesquisa))
            cmd.Parameters.AddWithValue("pesquisa", $"%{pesquisa}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var prescricao = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

            resultado.Add(new ConsultaResponse
            {
                Id = reader.GetGuid(0),
                Reason = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                FinalDiagnosis = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Date = reader.GetDateTime(3),
                Observations = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Medications = ParsearMedicamentos(prescricao),
                Doctor = new ConsultaDoctorDto
                {
                    Name = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Email = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Specialty = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    TypeProfissional = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                },
                Hospital = new ConsultaHospitalDto
                {
                    NameHospital = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    Cep = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    Street = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                    Number = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                    Complement = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                    Neighborhood = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                    City = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                    State = reader.IsDBNull(17) ? string.Empty : reader.GetString(17)
                }
            });
        }

        return resultado;
    }

    public async Task<ConsultaResponse> CriarAsync(CriarConsultaRequest request)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            INSERT INTO consultas (id_usuario, id_profissional, id_hospital, motivo_consulta, diagnostico, data_consulta, anotacoes_medicas, prescricao)
            VALUES (@id_usuario, @id_profissional, @id_hospital, @motivo_consulta, @diagnostico, @data_consulta, @anotacoes_medicas, @prescricao)
            RETURNING id, motivo_consulta, diagnostico, data_consulta, anotacoes_medicas, prescricao";

        cmd.Parameters.AddWithValue("id_usuario", request.IdPacient);
        cmd.Parameters.AddWithValue("id_profissional", request.IdDoctor);
        cmd.Parameters.AddWithValue("id_hospital", request.IdHospital);
        cmd.Parameters.AddWithValue("motivo_consulta", request.Reason);
        cmd.Parameters.AddWithValue("diagnostico", (object?)request.FinalDiagnosis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("data_consulta", request.Date);
        cmd.Parameters.AddWithValue("anotacoes_medicas", (object?)request.Observations ?? DBNull.Value);
        cmd.Parameters.AddWithValue("prescricao", (object?)request.Medications ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var prescricao = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

        return new ConsultaResponse
        {
            Id = reader.GetGuid(0),
            Reason = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
            FinalDiagnosis = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Date = reader.GetDateTime(3),
            Observations = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            Medications = ParsearMedicamentos(prescricao)
        };
    }

    private static List<ConsultaMedicationDto> ParsearMedicamentos(string prescricao)
    {
        if (string.IsNullOrWhiteSpace(prescricao))
            return new List<ConsultaMedicationDto>();

        return prescricao
            .Split(new[] { ',', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(m => new ConsultaMedicationDto { Name = m.Trim() })
            .Where(m => !string.IsNullOrWhiteSpace(m.Name))
            .ToList();
    }
}
