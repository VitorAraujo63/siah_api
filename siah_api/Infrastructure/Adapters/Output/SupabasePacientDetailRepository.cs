using Npgsql;
using SiahApi.Application.DTOs.PacientDetail;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabasePacientDetailRepository : IPacientDetailRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabasePacientDetailRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<PacientDetailResponse?> ObterPorCpfAsync(string cpf)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT
                id, nome, email, cpf, embedding_path, images, criado_em,
                rg, data_nascimento, genero, estado_civil, nacionalidade, naturalidade,
                telefone, telefone_secundario, cep, rua, numero, complemento, bairro,
                cidade, estado, hospital_vinculado, medico_responsavel, tipo_sanguineo,
                peso, altura, imc, pressao_arterial, frequencia_cardiaca,
                alergias, condicoes_cronicas, historico_familiar,
                possui_plano_saude, nome_plano, numero_carteirinha, validade_carteirinha,
                nome_responsavel, parentesco, telefone_responsavel
            FROM usuarios
            WHERE cpf = @cpf
            LIMIT 1";

        cmd.Parameters.AddWithValue("cpf", cpf);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new PacientDetailResponse
        {
            Id = reader.GetGuid(0),
            Nome = reader.IsDBNull(1) ? null : reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            Cpf = reader.GetString(3),
            EmbeddingPath = reader.IsDBNull(4) ? null : reader.GetString(4),
            Images = reader.IsDBNull(5) ? null : ((string[])reader[5]).ToList(),
            Embeddings = null,
            CriadoEm = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
            Rg = reader.IsDBNull(7) ? null : reader.GetString(7),
            DataNascimento = reader.IsDBNull(8) ? null : reader.GetFieldValue<DateOnly>(8).ToString("yyyy-MM-dd"),
            Genero = reader.IsDBNull(9) ? null : reader.GetString(9),
            EstadoCivil = reader.IsDBNull(10) ? null : reader.GetString(10),
            Nacionalidade = reader.IsDBNull(11) ? null : reader.GetString(11),
            Naturalidade = reader.IsDBNull(12) ? null : reader.GetString(12),
            Telefone = reader.IsDBNull(13) ? null : reader.GetString(13),
            TelefoneSecundario = reader.IsDBNull(14) ? null : reader.GetString(14),
            Cep = reader.IsDBNull(15) ? null : reader.GetString(15),
            Rua = reader.IsDBNull(16) ? null : reader.GetString(16),
            Numero = reader.IsDBNull(17) ? null : reader.GetString(17),
            Complemento = reader.IsDBNull(18) ? null : reader.GetString(18),
            Bairro = reader.IsDBNull(19) ? null : reader.GetString(19),
            Cidade = reader.IsDBNull(20) ? null : reader.GetString(20),
            Estado = reader.IsDBNull(21) ? null : reader.GetString(21),
            HospitalVinculado = reader.IsDBNull(22) ? null : reader.GetString(22),
            MedicoResponsavel = reader.IsDBNull(23) ? null : reader.GetString(23),
            TipoSanguineo = reader.IsDBNull(24) ? null : reader.GetString(24),
            Peso = reader.IsDBNull(25) ? null : reader.GetDecimal(25),
            Altura = reader.IsDBNull(26) ? null : reader.GetDecimal(26),
            Imc = reader.IsDBNull(27) ? null : reader.GetDecimal(27),
            PressaoArterial = reader.IsDBNull(28) ? null : reader.GetString(28),
            FrequenciaCardiaca = reader.IsDBNull(29) ? null : reader.GetString(29),
            Alergias = reader.IsDBNull(30) ? null : reader.GetString(30),
            CondicoesCronicas = reader.IsDBNull(31) ? null : reader.GetString(31),
            HistoricoFamiliar = reader.IsDBNull(32) ? null : reader.GetString(32),
            PossuiPlanoSaude = reader.IsDBNull(33) ? null : reader.GetBoolean(33),
            NomePlano = reader.IsDBNull(34) ? null : reader.GetString(34),
            NumeroCarteirinha = reader.IsDBNull(35) ? null : reader.GetString(35),
            ValidadeCarteirinha = reader.IsDBNull(36) ? null : reader.GetFieldValue<DateOnly>(36).ToString("yyyy-MM-dd"),
            NomeResponsavel = reader.IsDBNull(37) ? null : reader.GetString(37),
            Parentesco = reader.IsDBNull(38) ? null : reader.GetString(38),
            TelefoneResponsavel = reader.IsDBNull(39) ? null : reader.GetString(39)
        };
    }
}
