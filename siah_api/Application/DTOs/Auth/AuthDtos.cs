namespace SiahApi.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string ConfirmacaoSenha { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? DataNascimento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
}

public class RegisterResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public RegisterDataDto? Data { get; set; }
}

public class RegisterDataDto
{
    public Guid UserId { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Sucesso { get; set; }
    public LoginDataDto? Data { get; set; }
}

public class LoginDataDto
{
    public UsuarioResumoDto? User { get; set; }
}

public class UsuarioResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } = 3600;
}

public class CadastrarUsuarioTotemRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class CadastrarUsuarioTotemResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
}
