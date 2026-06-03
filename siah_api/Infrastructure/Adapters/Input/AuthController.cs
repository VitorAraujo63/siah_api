using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Auth;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("auth")]
[Tags("Autenticacao")]
public class AuthController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;

    public AuthController(IAuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    // Documentação de Integração_ SIAH API C#.pdf
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var resultado = await _authUseCase.RegisterAsync(request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { sucesso = false, error = "CPF_ALREADY_EXISTS", mensagem = "Este CPF já possui uma conta cadastrada." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    // Documentação de Integração_ SIAH API C#.pdf
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var resultado = await _authUseCase.LoginAsync(request);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { sucesso = false, mensagem = ex.Message });
        }
    }
}
