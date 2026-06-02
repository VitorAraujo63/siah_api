using SiahApi.Application.DTOs.Auth;

namespace SiahApi.Domain.Ports.Input;

public interface IAuthUseCase
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync(Guid userId, string token);
    Task<RefreshResponse> RefreshAsync(RefreshRequest request);
    Task<CadastrarUsuarioTotemResponse> CadastrarUsuarioTotemAsync(CadastrarUsuarioTotemRequest request);
}
