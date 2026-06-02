namespace SiahApi.Application.DTOs.Historico;

public class HistoricoResponse
{
    public Guid Id { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public MedicoHistoricoDto? Medico { get; set; }
    public string? Especialidade { get; set; }
}

public class MedicoHistoricoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}

public class HistoricoFiltros
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 20;
    public string? DateStart { get; set; }
    public string? DateEnd { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? SpecialtyId { get; set; }
    public string? Status { get; set; }
}
