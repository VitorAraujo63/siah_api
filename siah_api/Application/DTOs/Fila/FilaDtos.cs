namespace SiahApi.Application.DTOs.Fila;

public class ValidarTotemRequest
{
    public string PatientCpf { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
}

public class EmitirSenhaResponse
{
    public bool Sucesso { get; set; }
    public EmitirSenhaDataDto? Data { get; set; }
}

public class EmitirSenhaDataDto
{
    public long TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class SenhaAtivaResponse
{
    public long TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public DateTime? CalledAt { get; set; }
}

public class StatusSenhaResponse
{
    public long TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
}

public class ConfirmarPresencaRequest
{
    public long TicketId { get; set; }
}
