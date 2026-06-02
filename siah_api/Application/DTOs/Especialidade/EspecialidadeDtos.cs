namespace SiahApi.Application.DTOs.Especialidade;

public class EspecialidadeResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

public class MedicoSummaryResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public decimal Rating { get; set; }
}
