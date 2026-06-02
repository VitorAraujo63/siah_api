using SiahApi.Application.DTOs.Auth;
using SiahApi.Application.Services;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class AuthUseCase : IAuthUseCase
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthUseCase(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (request.Senha != request.ConfirmacaoSenha)
            throw new ArgumentException("As senhas não conferem.");

        var existe = await _authRepository.ExistePorCpfAsync(request.Cpf);
        if (existe)
            throw new InvalidOperationException("CPF_ALREADY_EXISTS");

        var paciente = new Paciente
        {
            Nome = request.Nome,
            Cpf = request.Cpf,
            Email = request.Email,
            Telefone = request.Telefone,
            DataNascimento = request.DataNascimento,
            SenhaHash = BCrypt(request.Senha)
        };

        var criado = await _authRepository.CriarAsync(paciente);

        return new RegisterResponse
        {
            Sucesso = true,
            Mensagem = "Conta criada com sucesso.",
            Data = new RegisterDataDto
            {
                UserId = criado.Id,
                Cpf = criado.Cpf,
                Nome = criado.Nome
            }
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var paciente = await _authRepository.ObterPorCpfAsync(request.Cpf);
        if (paciente is null || !VerificarSenha(request.Senha, paciente.SenhaHash))
            throw new UnauthorizedAccessException("CPF ou senha inválidos.");

        var accessToken = _jwtService.GerarAccessToken(paciente.Id, paciente.Cpf, paciente.Nome);
        var refreshToken = _jwtService.GerarRefreshToken();
        var expiracao = DateTime.UtcNow.AddDays(30);

        await _authRepository.SalvarRefreshTokenAsync(paciente.Id, refreshToken, expiracao);

        return new LoginResponse
        {
            Sucesso = true,
            Data = new LoginDataDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600,
                User = new UsuarioResumoDto
                {
                    Id = paciente.Id,
                    Nome = paciente.Nome,
                    Cpf = paciente.Cpf
                }
            }
        };
    }

    public async Task LogoutAsync(Guid userId, string token)
    {
        await _authRepository.InvalidarTokenAsync(userId, token);
    }

    public async Task<RefreshResponse> RefreshAsync(RefreshRequest request)
    {
        var (userId, valido) = await _authRepository.ValidarRefreshTokenAsync(request.RefreshToken);
        if (!valido)
            throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");

        var paciente = await _authRepository.ObterPorIdAsync(userId);
        if (paciente is null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        await _authRepository.RevogarRefreshTokenAsync(request.RefreshToken);

        var novoAccessToken = _jwtService.GerarAccessToken(paciente.Id, paciente.Cpf, paciente.Nome);
        var novoRefreshToken = _jwtService.GerarRefreshToken();
        await _authRepository.SalvarRefreshTokenAsync(paciente.Id, novoRefreshToken, DateTime.UtcNow.AddDays(30));

        return new RefreshResponse
        {
            AccessToken = novoAccessToken,
            RefreshToken = novoRefreshToken,
            ExpiresIn = 3600
        };
    }

    public async Task<CadastrarUsuarioTotemResponse> CadastrarUsuarioTotemAsync(CadastrarUsuarioTotemRequest request)
    {
        var paciente = new Paciente
        {
            Nome = request.Nome,
            Cpf = request.Cpf,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email
        };

        var resultado = await _authRepository.CriarOuAtualizarTotemAsync(paciente);

        return new CadastrarUsuarioTotemResponse
        {
            Id = resultado.Id,
            Nome = resultado.Nome,
            Cpf = resultado.Cpf
        };
    }

    private static string BCrypt(string senha) => senha;
    private static bool VerificarSenha(string senha, string? hash) => senha == hash;
}
