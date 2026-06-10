using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace SiahApi.Application.DTOs.Vacina;

public class VacinaResponse
{
    public Guid Id { get; set; }
    public string NomeVacina { get; set; } = string.Empty;
    public string DataAplicacao { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string Lote { get; set; } = string.Empty;
    public Guid IdHospital { get; set; }
    public Guid IdProfissional { get; set; }
    public VacinaDoctorDto? Doutor { get; set; }
    public VacinaHospitalDto? Hospital { get; set; }
}

public class VacinaDoctorDto
{
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string TypeProfissional { get; set; } = string.Empty;
}

public class VacinaHospitalDto
{
    public string Cep { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string NameHospital { get; set; } = string.Empty;
}

public class CriarVacinaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string NomeVacina { get; set; } = string.Empty;
    public string DataAplicacao { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string Lote { get; set; } = string.Empty;
    public Guid IdHospital { get; set; }
    public Guid IdProfissional { get; set; }
}

public class AtualizarVacinaRequest
{
    public string? NomeVacina { get; set; }
    public string? DataAplicacao { get; set; }
    public string? Dose { get; set; }
    public string? Lote { get; set; }
    public Guid? IdHospital { get; set; }
    public Guid? IdProfissional { get; set; }
}
