using SiahApi.Application.DTOs.Documento;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class DocumentoUseCase : IDocumentoUseCase
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IAuthRepository _authRepository;

    public DocumentoUseCase(IDocumentoRepository documentoRepository, IAuthRepository authRepository)
    {
        _documentoRepository = documentoRepository;
        _authRepository = authRepository;
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarAtestadosAsync(string cpf)
        => await ListarPorTipoAsync(cpf, "certificate");

    public async Task<byte[]> ObterAtestadoPdfAsync(string cpf, Guid documentoId)
    {
        var pdf = await _documentoRepository.ObterPdfAsync(documentoId);
        if (pdf is null)
            throw new KeyNotFoundException("PDF do atestado não encontrado.");
        return pdf;
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarExamesAsync(string cpf)
        => await ListarPorTipoAsync(cpf, "exam");

    public async Task<DocumentoResponse> ObterResultadoExameAsync(string cpf, Guid exameId)
    {
        var exame = await _documentoRepository.ObterPorIdAsync(exameId)
            ?? throw new KeyNotFoundException("Exame não encontrado.");

        return MapearParaResponse(exame);
    }

    public async Task<IEnumerable<DocumentoResponse>> ListarReceitasAsync(string cpf)
        => await ListarPorTipoAsync(cpf, "prescription");

    public async Task<IEnumerable<DocumentoResponse>> ListarReceitasAtivasAsync(string cpf)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var receitas = await _documentoRepository.ListarPorTipoAsync(userId, "prescription");
        return receitas.Where(r => r.Ativo).Select(MapearParaResponse);
    }

    public async Task<IEnumerable<DocumentoResponse>> BuscarAsync(string cpf, string termo)
    {
        var userId = await ResolverUserIdAsync(cpf);
        var documentos = await _documentoRepository.BuscarAsync(userId, termo);
        return documentos.Select(MapearParaResponse);
    }

    private async Task<Guid> ResolverUserIdAsync(string cpf)
    {
        var paciente = await _authRepository.ObterPorCpfAsync(cpf)
            ?? throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");
        return paciente.Id;
    }

    private async Task<IEnumerable<DocumentoResponse>> ListarPorTipoAsync(string cpf, string tipo)
    {
        var userId = await ResolverUserIdAsync(cpf);
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
