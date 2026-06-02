using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Auth;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

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

    // SIAH_Especificacao_API_v1.docx
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    // SIAH_Especificacao_API_v1.docx
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
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _authUseCase.LogoutAsync(userId, token);
        return Ok(new { mensagem = "Logout realizado com sucesso." });
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var resultado = await _authUseCase.RefreshAsync(request);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}
