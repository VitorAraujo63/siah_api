namespace SiahApi.Application.DTOs.Medico;

public class MedicoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public decimal Rating { get; set; }
    public bool DisponivelHoje { get; set; }
}

public class MedicoFiltros
{
    public Guid? SpecialtyId { get; set; }
    public Guid? HospitalId { get; set; }
    public decimal? RatingMin { get; set; }
    public bool? AvailableToday { get; set; }
    public Guid? InsuranceId { get; set; }
    public string? Name { get; set; }
    public string? SortBy { get; set; }
}

public class DisponibilidadeResponse
{
    public Guid DoctorId { get; set; }
    public List<SlotDto> Slots { get; set; } = new();
}

public class SlotDto
{
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public bool Available { get; set; }
}

public class DisponibilidadeFiltros
{
    public string? DateStart { get; set; }
    public string? DateEnd { get; set; }
    public Guid? SpecialtyId { get; set; }
}

public class AvaliacaoResponse
{
    public string Avaliador { get; set; } = string.Empty;
    public string? Comentario { get; set; }
    public decimal Nota { get; set; }
    public DateTime Data { get; set; }
}
