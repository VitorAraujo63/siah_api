using SiahApi.Application.DTOs.Documento;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class DocumentoUseCase : IDocumentoUseCase
{
    private readonly IDocumentoRepository _documentoRepository;

    public DocumentoUseCase(IDocumentoRepository documentoRepository)
    {
        _documentoRepository = documentoRepository;
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarAtestadosAsync(Guid userId)
        => await ListarPorTipoAsync(userId, "certificate");

    public async Task<byte[]> ObterAtestadoPdfAsync(Guid userId, Guid documentoId)
    {
        var pdf = await _documentoRepository.ObterPdfAsync(documentoId);
        if (pdf is null)
            throw new KeyNotFoundException("PDF do atestado não encontrado.");
        return pdf;
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarExamesAsync(Guid userId)
        => await ListarPorTipoAsync(userId, "exam");

    public async Task<DocumentoResponse> ObterResultadoExameAsync(Guid userId, Guid exameId)
    {
        var exame = await _documentoRepository.ObterPorIdAsync(exameId)
            ?? throw new KeyNotFoundException("Exame não encontrado.");

        return MapearParaResponse(exame);
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarReceitasAsync(Guid userId)
        => await ListarPorTipoAsync(userId, "prescription");

    public async Task<IEnumerable<DocumentoResponse>> ListarReceitasAtivasAsync(Guid userId)
    {
        var receitas = await _documentoRepository.ListarPorTipoAsync(userId, "prescription");
        return receitas.Where(r => r.Ativo).Select(MapearParaResponse);
    }

    public async Task<IEnumerable<DocumentoResponse>> BuscarAsync(Guid userId, string termo)
    {
        var documentos = await _documentoRepository.BuscarAsync(userId, termo);
        return documentos.Select(MapearParaResponse);
    }

    private async Task<IEnumerable<DocumentoResponse>> ListarPorTipoAsync(Guid userId, string tipo)
    {
        var documentos = await _documentoRepository.ListarPorTipoAsync(userId, tipo);
        return documentos.Select(MapearParaResponse);
    }

    private static DocumentoResponse MapearParaResponse(Domain.Entities.Documento d) => new()
    {
        Id = d.Id,
        Tipo = d.Tipo,
        Titulo = d.Titulo,
        Conteudo = d.Conteudo,
        PdfUrl = d.PdfUrl,
        AgendamentoId = d.AgendamentoId,
        Ativo = d.Ativo,
        CriadoEm = d.CriadoEm
    };
}
