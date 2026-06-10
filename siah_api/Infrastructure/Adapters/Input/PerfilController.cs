using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Perfil;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("profile")]
[Tags("Perfil")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilUseCase _perfilUseCase;

    public PerfilController(IPerfilUseCase perfilUseCase)
    {
        _perfilUseCase = perfilUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter([FromQuery] string cpf)
    {
        var resultado = await _perfilUseCase.ObterAsync(cpf);
        return Ok(resultado);
    }

    [HttpPut]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarPerfilRequest request, [FromQuery] string cpf)
    {
        var resultado = await _perfilUseCase.AtualizarAsync(cpf, request);
        return Ok(resultado);
    }

    [HttpPatch("basic-info")]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarDadosBasicos([FromBody] AtualizarDadosBasicosRequest request, [FromQuery] string cpf)
    {
        var resultado = await _perfilUseCase.AtualizarDadosBasicosAsync(cpf, request);
        return Ok(resultado);
    }

    [HttpPatch("photo")]
    [ProducesResponseType(typeof(AtualizarFotoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarFoto(IFormFile photo, [FromQuery] string cpf)
    {
        var resultado = await _perfilUseCase.AtualizarFotoAsync(cpf, photo);
        return Ok(resultado);
    }

    [HttpDelete("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoverFoto([FromQuery] string cpf)
    {
        await _perfilUseCase.RemoverFotoAsync(cpf);
        return Ok(new { mensagem = "Foto removida com sucesso." });
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SolicitarExclusao([FromQuery] string cpf)
    {
        await _perfilUseCase.SolicitarExclusaoAsync(cpf);
        return Ok(new { mensagem = "Solicitação de exclusão registrada. Você receberá um e-mail de confirmação." });
    }
}
