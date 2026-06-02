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

    // documentacao.pdf
    [HttpPost]
    [ProducesResponseType(typeof(CadastrarUsuarioTotemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CadastrarTotem([FromBody] CadastrarUsuarioTotemRequest request)
    {
        var resultado = await _authUseCase.CadastrarUsuarioTotemAsync(request);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }
}
