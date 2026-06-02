using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Perfil;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("profile")]
[Authorize]
[Tags("Perfil")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilUseCase _perfilUseCase;

    public PerfilController(IPerfilUseCase perfilUseCase)
    {
        _perfilUseCase = perfilUseCase;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // SIAH_Especificacao_API_v1.docx
    [HttpGet]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter()
    {
        var resultado = await _perfilUseCase.ObterAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPut]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarPerfilRequest request)
    {
        var resultado = await _perfilUseCase.AtualizarAsync(UserId, request);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPatch("basic-info")]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarDadosBasicos([FromBody] AtualizarDadosBasicosRequest request)
    {
        var resultado = await _perfilUseCase.AtualizarDadosBasicosAsync(UserId, request);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPatch("photo")]
    [ProducesResponseType(typeof(AtualizarFotoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarFoto(IFormFile photo)
    {
        var resultado = await _perfilUseCase.AtualizarFotoAsync(UserId, photo);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpDelete("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoverFoto()
    {
        await _perfilUseCase.RemoverFotoAsync(UserId);
        return Ok(new { mensagem = "Foto removida com sucesso." });
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SolicitarExclusao()
    {
        await _perfilUseCase.SolicitarExclusaoAsync(UserId);
        return Ok(new { mensagem = "Solicitação de exclusão registrada. Você receberá um e-mail de confirmação." });
    }
}
