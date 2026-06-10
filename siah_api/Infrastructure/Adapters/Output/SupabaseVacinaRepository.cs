using Npgsql;
using NpgsqlTypes;
using SiahApi.Application.DTOs.Vacina;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseVacinaRepository : IVacinaRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseVacinaRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<VacinaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null)
    {
        var resultado = new List<VacinaResponse>();

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        var filtro = string.IsNullOrWhiteSpace(pesquisa)
            ? string.Empty
            : "AND v.nome_vacina ILIKE @pesquisa";

        cmd.CommandText = $@"
            SELECT v.id, v.nome_vacina, v.data_aplicacao::text, v.dose, v.lote,
                   v.id_hospital, v.id_profissional,
                   p.nome, p.especialidade, p.tipo_profissional,
                   h.cep, h.rua, h.bairro, h.cidade, h.estado, h.numero, h.complemento, h.nome_hospital
            FROM vacinas v
            INNER JOIN usuarios u ON u.id = v.id_usuario
            LEFT JOIN profissionais p ON v.id_profissional = p.id
            LEFT JOIN hospitais h ON v.id_hospital = h.id
            WHERE u.cpf = @cpf
            {filtro}
            ORDER BY v.data_aplicacao DESC";
        cmd.Parameters.AddWithValue("cpf", cpf);

        if (!string.IsNullOrWhiteSpace(pesquisa))
            cmd.Parameters.AddWithValue("pesquisa", $"%{pesquisa}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new VacinaResponse
            {
                Id = reader.GetGuid(0),
                NomeVacina = reader.GetString(1),
                DataAplicacao = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Dose = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Lote = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                IdHospital = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5),
                IdProfissional = reader.IsDBNull(6) ? Guid.Empty : reader.GetGuid(6),
                Doutor = new VacinaDoctorDto
                {
                    Nome = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Especialidade = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    TypeProfissional = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                },
                Hospital = new VacinaHospitalDto
                {
                    Cep = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    Rua = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    Bairro = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                    Cidade = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                    Estado = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                    Numero = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                    Complemento = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                    NameHospital = reader.IsDBNull(17) ? string.Empty : reader.GetString(17)
                }
            });
        }

        return resultado;
    }

    public async Task<VacinaResponse> CriarAsync(CriarVacinaRequest request)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmdUsuario = conn.CreateCommand();
        cmdUsuario.CommandText = "SELECT id FROM usuarios WHERE cpf = @cpf LIMIT 1";
        cmdUsuario.Parameters.AddWithValue("cpf", request.Cpf);

        var idUsuario = (Guid?)await cmdUsuario.ExecuteScalarAsync()
            ?? throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO vacinas (id_usuario, nome_vacina, data_aplicacao, dose, lote, id_hospital, id_profissional)
            VALUES (@id_usuario, @nome_vacina, @data_aplicacao, @dose, @lote, @id_hospital, @id_profissional)
            RETURNING id, nome_vacina, data_aplicacao::text, dose, lote, id_hospital, id_profissional";

        cmd.Parameters.AddWithValue("id_usuario", idUsuario);
        cmd.Parameters.AddWithValue("nome_vacina", request.NomeVacina);
        cmd.Parameters.Add(new NpgsqlParameter("data_aplicacao", NpgsqlDbType.Date)
        {
            Value = !string.IsNullOrEmpty(request.DataAplicacao)
                ? DateOnly.Parse(request.DataAplicacao)
                : DBNull.Value
        });
        cmd.Parameters.AddWithValue("dose", (object?)request.Dose ?? DBNull.Value);
        cmd.Parameters.AddWithValue("lote", (object?)request.Lote ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id_hospital", request.IdHospital);
        cmd.Parameters.AddWithValue("id_profissional", request.IdProfissional);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new VacinaResponse
        {
            Id = reader.GetGuid(0),
            NomeVacina = reader.GetString(1),
            DataAplicacao = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Dose = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Lote = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            IdHospital = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5),
            IdProfissional = reader.IsDBNull(6) ? Guid.Empty : reader.GetGuid(6)
        };
    }

    public async Task<VacinaResponse?> AtualizarAsync(Guid id, AtualizarVacinaRequest request)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmdAtual = conn.CreateCommand();
        cmdAtual.CommandText = @"
            SELECT id, nome_vacina, data_aplicacao::text, dose, lote, id_hospital, id_profissional
            FROM vacinas WHERE id = @id LIMIT 1";
        cmdAtual.Parameters.AddWithValue("id", id);

        await using var readerAtual = await cmdAtual.ExecuteReaderAsync();
        if (!await readerAtual.ReadAsync()) return null;

        var atual = new VacinaResponse
        {
            Id = readerAtual.GetGuid(0),
            NomeVacina = readerAtual.GetString(1),
            DataAplicacao = readerAtual.IsDBNull(2) ? string.Empty : readerAtual.GetString(2),
            Dose = readerAtual.IsDBNull(3) ? string.Empty : readerAtual.GetString(3),
            Lote = readerAtual.IsDBNull(4) ? string.Empty : readerAtual.GetString(4),
            IdHospital = readerAtual.IsDBNull(5) ? Guid.Empty : readerAtual.GetGuid(5),
            IdProfissional = readerAtual.IsDBNull(6) ? Guid.Empty : readerAtual.GetGuid(6)
        };
        await readerAtual.CloseAsync();

        var nomeVacina = request.NomeVacina ?? atual.NomeVacina;
        var dataAplicacao = request.DataAplicacao ?? atual.DataAplicacao;
        var dose = request.Dose ?? atual.Dose;
        var lote = request.Lote ?? atual.Lote;
        var idHospital = request.IdHospital ?? atual.IdHospital;
        var idProfissional = request.IdProfissional ?? atual.IdProfissional;

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE vacinas SET
                nome_vacina = @nome_vacina,
                data_aplicacao = @data_aplicacao,
                dose = @dose,
                lote = @lote,
                id_hospital = @id_hospital,
                id_profissional = @id_profissional
            WHERE id = @id
            RETURNING id, nome_vacina, data_aplicacao::text, dose, lote, id_hospital, id_profissional";

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("nome_vacina", nomeVacina);
        cmd.Parameters.Add(new NpgsqlParameter("data_aplicacao", NpgsqlDbType.Date)
        {
            Value = !string.IsNullOrEmpty(dataAplicacao)
                ? DateOnly.Parse(dataAplicacao)
                : DBNull.Value
        });
        cmd.Parameters.AddWithValue("dose", (object?)dose ?? DBNull.Value);
        cmd.Parameters.AddWithValue("lote", (object?)lote ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id_hospital", idHospital);
        cmd.Parameters.AddWithValue("id_profissional", idProfissional);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new VacinaResponse
        {
            Id = reader.GetGuid(0),
            NomeVacina = reader.GetString(1),
            DataAplicacao = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Dose = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Lote = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            IdHospital = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5),
            IdProfissional = reader.IsDBNull(6) ? Guid.Empty : reader.GetGuid(6)
        };
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM vacinas WHERE id = @id";
        cmd.Parameters.AddWithValue("id", id);
        var linhasAfetadas = await cmd.ExecuteNonQueryAsync();
        return linhasAfetadas > 0;
    }
}
