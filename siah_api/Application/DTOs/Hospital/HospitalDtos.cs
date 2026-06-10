namespace SiahApi.Application.DTOs.Hospital;

public class HospitalResponse
{
    public Guid Id { get; set; }
    public string NomeHospital { get; set; } = string.Empty;
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}
