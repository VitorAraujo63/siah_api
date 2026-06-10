namespace SiahApi.Application.DTOs.Rag;

public class RagPatientDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
}

public class RagMedicationDto
{
    public string Name { get; set; } = string.Empty;
}

public class RagConsultationDto
{
    public string Id { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string FinalDiagnosis { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Observations { get; set; } = string.Empty;
    public List<RagMedicationDto> Medications { get; set; } = new();
}
