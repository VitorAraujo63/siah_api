namespace SiahApi.Application.DTOs.Paciente;

public class CadastrarPacienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? DataNascimento { get; set; }
    public string? Genero { get; set; }
    public string? TipoSanguineo { get; set; }
    public string? HospitalVinculado { get; set; }
    public string? Rg { get; set; }
    public string? CartaoSus { get; set; }
    public string? Cnh { get; set; }
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public bool PossuiPlanoSaude { get; set; }
    public string? NomePlano { get; set; }
    public string? NumeroCarteirinha { get; set; }
    public string? ValidadeCarteirinha { get; set; }
    public string? NomeResponsavel { get; set; }
    public string? Parentesco { get; set; }
    public string? TelefoneResponsavel { get; set; }
    public List<string> Images { get; set; } = new();
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public string? EmbeddingPath { get; set; }
    public string? TempFile { get; set; }
}

public class CadastrarPacienteResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? EmbeddingPath { get; set; }
    public List<string> Images { get; set; } = new();
}

public class ReconhecerPacienteRequest
{
    public List<string> Images { get; set; } = new();
}

public class ReconhecerPacienteResponse
{
    public bool Sucesso { get; set; }
    public PacienteReconhecidoDto? Paciente { get; set; }
}

public class PacienteReconhecidoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public double Distancia { get; set; }
}
