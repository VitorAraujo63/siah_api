namespace SiahApi.Application.DTOs.Fila;

public class ValidarTotemRequest
{
    public string PatientCpf { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string TotemId { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
}

public class EmitirSenhaResponse
{
    public bool Sucesso { get; set; }
    public EmitirSenhaDataDto? Data { get; set; }
}

public class EmitirSenhaDataDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public string Department { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}

public class SenhaAtivaResponse
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public DateTime? CalledAt { get; set; }
    public string Department { get; set; } = string.Empty;
}

public class StatusSenhaResponse
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public int EstimatedWaitMinutes { get; set; }
}

public class ConfirmarPresencaRequest
{
    public Guid TicketId { get; set; }
}
