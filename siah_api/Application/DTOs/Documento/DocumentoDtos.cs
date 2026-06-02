namespace SiahApi.Application.DTOs.Documento;

public class DocumentoResponse
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public string? PdfUrl { get; set; }
    public Guid? AgendamentoId { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
