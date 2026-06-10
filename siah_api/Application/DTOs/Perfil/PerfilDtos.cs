namespace SiahApi.Application.DTOs.Perfil;

public class PerfilResponse
{
    public Guid Id { get; set; }
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
    public string? FotoUrl { get; set; }
    public EnderecoDto? Endereco { get; set; }
    public PlanoSaudeDto? PlanoSaude { get; set; }
    public ResponsavelDto? Responsavel { get; set; }
}

public class EnderecoDto
{
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class PlanoSaudeDto
{
    public bool Possui { get; set; }
    public string? Nome { get; set; }
    public string? NumeroCarteirinha { get; set; }
    public string? Validade { get; set; }
}

public class ResponsavelDto
{
    public string? Nome { get; set; }
    public string? Parentesco { get; set; }
    public string? Telefone { get; set; }
}

public class AtualizarPerfilRequest
{
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? DataNascimento { get; set; }
    public string? Genero { get; set; }
    public string? TipoSanguineo { get; set; }
    public string? Rg { get; set; }
    public string? CartaoSus { get; set; }
    public string? Cnh { get; set; }
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public bool? PossuiPlanoSaude { get; set; }
    public string? NomePlano { get; set; }
    public string? NumeroCarteirinha { get; set; }
    public string? ValidadeCarteirinha { get; set; }
    public string? NomeResponsavel { get; set; }
    public string? Parentesco { get; set; }
    public string? TelefoneResponsavel { get; set; }
}

public class AtualizarDadosBasicosRequest
{
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class AtualizarFotoResponse
{
    public string FotoUrl { get; set; } = string.Empty;
}
