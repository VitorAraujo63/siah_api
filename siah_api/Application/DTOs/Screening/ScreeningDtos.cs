namespace SiahApi.Application.DTOs.Screening;

public class ScreeningResponse
{
    public Guid Id { get; set; }
    public string? BloodPressure { get; set; }
    public string? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public DateTime? DateScreening { get; set; }
}

public class CriarScreeningRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string? BloodPressure { get; set; }
    public string? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public DateTime? DateScreening { get; set; }
}
