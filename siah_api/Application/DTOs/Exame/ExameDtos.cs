namespace SiahApi.Application.DTOs.Exame;

public class ExameResponse
{
    public Guid Id { get; set; }
    public string Exam { get; set; } = string.Empty;
    public string DateExam { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
    public string? NameLaboratory { get; set; }
}

public class CriarExameRequest
{
    public Guid IdPacient { get; set; }
    public string Exam { get; set; } = string.Empty;
    public string DateExam { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
    public string? NameLaboratory { get; set; }
}
