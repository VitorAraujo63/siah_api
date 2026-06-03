using SiahApi.Application.DTOs.Auth;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class AuthUseCase : IAuthUseCase
{
    private readonly IAuthRepository _authRepository;

    public AuthUseCase(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
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
            SenhaHash = request.Senha
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
        if (paciente is null || paciente.SenhaHash != request.Senha)
            throw new UnauthorizedAccessException("CPF ou senha inválidos.");

        return new LoginResponse
        {
            Sucesso = true,
            Data = new LoginDataDto
            {
                User = new UsuarioResumoDto
                {
                    Id = paciente.Id,
                    Nome = paciente.Nome,
                    Cpf = paciente.Cpf
                }
            }
        };
    }

    public Task LogoutAsync(Guid userId, string token) => Task.CompletedTask;

    public Task<RefreshResponse> RefreshAsync(RefreshRequest request) =>
        throw new NotImplementedException();

    public async Task<CadastrarUsuarioTotemResponse?> ObterUsuarioTotemPorCpfAsync(string cpf)
    {
        var paciente = await _authRepository.ObterPorCpfAsync(cpf);
        if (paciente == null) return null;

        return new CadastrarUsuarioTotemResponse
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Cpf = paciente.Cpf
        };
    }
}
