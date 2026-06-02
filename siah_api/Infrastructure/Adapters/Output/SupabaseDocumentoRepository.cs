using Npgsql;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Infrastructure.Adapters.Output;

public class SupabaseDocumentoRepository : IDocumentoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SupabaseDocumentoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Documento>> ListarPorTipoAsync(Guid userId, string tipo)
    {
        var resultado = new List<Documento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, tipo, titulo, conteudo, pdf_url, agendamento_id, ativo, criado_em
            FROM documentos WHERE id_usuario = @id_usuario AND tipo = @tipo ORDER BY criado_em DESC";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("tipo", tipo);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearDocumento(reader));

        return resultado;
    }

    public async Task<Documento?> ObterPorIdAsync(Guid documentoId)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, tipo, titulo, conteudo, pdf_url, agendamento_id, ativo, criado_em
            FROM documentos WHERE id = @id LIMIT 1";
        cmd.Parameters.AddWithValue("id", documentoId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return MapearDocumento(reader);
    }

    public Task<byte[]?> ObterPdfAsync(Guid documentoId)
    {
        return Task.FromResult<byte[]?>(null);
    }

    public async Task<IEnumerable<Documento>> BuscarAsync(Guid userId, string termo)
    {
        var resultado = new List<Documento>();
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, id_usuario, tipo, titulo, conteudo, pdf_url, agendamento_id, ativo, criado_em
            FROM documentos
            WHERE id_usuario = @id_usuario AND (titulo ILIKE @termo OR conteudo ILIKE @termo)
            ORDER BY criado_em DESC";
        cmd.Parameters.AddWithValue("id_usuario", userId);
        cmd.Parameters.AddWithValue("termo", $"%{termo}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearDocumento(reader));

        return resultado;
    }

    private static Documento MapearDocumento(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetGuid(0),
        IdUsuario = reader.GetGuid(1),
        Tipo = reader.GetString(2),
        Titulo = reader.GetString(3),
        Conteudo = reader.IsDBNull(4) ? null : reader.GetString(4),
        PdfUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
        AgendamentoId = reader.IsDBNull(6) ? null : reader.GetGuid(6),
        Ativo = reader.GetBoolean(7),
        CriadoEm = reader.GetDateTime(8)
    };
}
