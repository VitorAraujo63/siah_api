namespace SiahApi.Domain.Entities;

public class Triagem
{
    public Guid Id { get; set; }
    public Guid IdUsuario { get; set; }
    public string QueixaPrincipal { get; set; } = string.Empty;
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    public string? Temperatura { get; set; }
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
