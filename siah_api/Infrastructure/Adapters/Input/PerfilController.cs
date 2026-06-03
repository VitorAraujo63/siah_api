using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Perfil;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

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

    // SIAH_Especificacao_API_v1.docx
    [HttpGet]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter([FromQuery] Guid userId)
    {
        var resultado = await _perfilUseCase.ObterAsync(userId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPut]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarPerfilRequest request, [FromQuery] Guid userId)
    {
        var resultado = await _perfilUseCase.AtualizarAsync(userId, request);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPatch("basic-info")]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarDadosBasicos([FromBody] AtualizarDadosBasicosRequest request, [FromQuery] Guid userId)
    {
        var resultado = await _perfilUseCase.AtualizarDadosBasicosAsync(userId, request);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPatch("photo")]
    [ProducesResponseType(typeof(AtualizarFotoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarFoto(IFormFile photo, [FromQuery] Guid userId)
    {
        var resultado = await _perfilUseCase.AtualizarFotoAsync(userId, photo);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpDelete("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoverFoto([FromQuery] Guid userId)
    {
        await _perfilUseCase.RemoverFotoAsync(userId);
        return Ok(new { mensagem = "Foto removida com sucesso." });
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SolicitarExclusao([FromQuery] Guid userId)
    {
        await _perfilUseCase.SolicitarExclusaoAsync(userId);
        return Ok(new { mensagem = "Solicitação de exclusão registrada. Você receberá um e-mail de confirmação." });
    }
}
