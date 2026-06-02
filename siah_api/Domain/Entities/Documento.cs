namespace SiahApi.Domain.Entities;

public class Documento
{
    public Guid Id { get; set; }
    public Guid IdUsuario { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public string? PdfUrl { get; set; }
    public Guid? AgendamentoId { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
