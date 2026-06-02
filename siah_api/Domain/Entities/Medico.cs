namespace SiahApi.Domain.Entities;

public class Medico
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public Guid? HospitalId { get; set; }
    public string? FotoUrl { get; set; }
    public decimal Rating { get; set; }
    public bool DisponivelHoje { get; set; }
    public List<Guid> PlanoIds { get; set; } = new();
}
