using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Auth;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("api/usuarios")]
[Tags("Usuarios (Totem)")]
public class UsuariosController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;

    public UsuariosController(IAuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(CadastrarUsuarioTotemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterUsuario(string cpf)
    {
        var resultado = await _authUseCase.ObterUsuarioTotemPorCpfAsync(cpf);
        if (resultado == null) return NotFound(new { erro = "Usuário não encontrado." });
        
        return Ok(resultado);
    }
}
