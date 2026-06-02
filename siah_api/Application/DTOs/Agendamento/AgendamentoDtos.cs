namespace SiahApi.Application.DTOs.Agendamento;

public class AgendarRequest
{
    public Guid DoctorId { get; set; }
    public Guid SpecialtyId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? InsuranceId { get; set; }
}

public class ReagendarRequest
{
    public string Date { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
}

public class CancelarRequest
{
    public string? Reason { get; set; }
    public string CancelType { get; set; } = "patient";
}

public class AgendamentoResponse
{
    public Guid AppointmentId { get; set; }
    public MedicoAgendamentoDto? Doctor { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? QueueNumber { get; set; }
    public bool? RefundEligible { get; set; }
    public DateTime? CancelledAt { get; set; }
}

public class MedicoAgendamentoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}

public class AgendamentoFiltros
{
    public string? Status { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? SpecialtyId { get; set; }
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 20;
}
