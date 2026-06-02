namespace SiahApi.Application.DTOs.Triagem;

public class RegistrarTriagemRequest
{
    public string CpfPaciente { get; set; } = string.Empty;
    public string QueixaPrincipal { get; set; } = string.Empty;
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    public string? Temperatura { get; set; }
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
}

public class RegistrarTriagemResponse
{
    public string Status { get; set; } = "sucesso";
    public TriagemDataDto Data { get; set; } = new();
}

public class TriagemDataDto
{
    public Guid Id { get; set; }
    public Guid IdUsuario { get; set; }
    public string QueixaPrincipal { get; set; } = string.Empty;
    public string? Peso { get; set; }
    public string? Altura { get; set; }
    public string? Temperatura { get; set; }
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
}
