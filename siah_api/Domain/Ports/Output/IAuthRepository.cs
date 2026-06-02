using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IAuthRepository
{
    Task<bool> ExistePorCpfAsync(string cpf);
    Task<Paciente> CriarAsync(Paciente paciente);
    Task<Paciente?> ObterPorCpfAsync(string cpf);
    Task<Paciente?> ObterPorIdAsync(Guid id);
    Task<Paciente> AtualizarAsync(Paciente paciente);
    Task InvalidarTokenAsync(Guid userId, string token);
    Task<bool> TokenEstaInvalidadoAsync(string token);
    Task SalvarRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiracao);
    Task<(Guid UserId, bool Valido)> ValidarRefreshTokenAsync(string refreshToken);
    Task RevogarRefreshTokenAsync(string refreshToken);
    Task<Paciente> CriarOuAtualizarTotemAsync(Paciente paciente);
}
