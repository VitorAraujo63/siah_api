namespace SiahApi.Application.DTOs.Consulta;

public class ConsultaResponse
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string FinalDiagnosis { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Observations { get; set; } = string.Empty;
    public List<ConsultaMedicationDto> Medications { get; set; } = new();
    public ConsultaDoctorDto? Doctor { get; set; }
    public ConsultaHospitalDto? Hospital { get; set; }
}

public class ConsultaMedicationDto
{
    public string Name { get; set; } = string.Empty;
}

public class ConsultaDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string TypeProfissional { get; set; } = string.Empty;
}

public class ConsultaHospitalDto
{
    public string Cep { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Complement { get; set; } = string.Empty;
    public string NameHospital { get; set; } = string.Empty;
}

public class CriarConsultaRequest
{
    public Guid IdPacient { get; set; }
    public Guid IdDoctor { get; set; }
    public Guid IdHospital { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string FinalDiagnosis { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Observations { get; set; } = string.Empty;
    public string Medications { get; set; } = string.Empty;
}
