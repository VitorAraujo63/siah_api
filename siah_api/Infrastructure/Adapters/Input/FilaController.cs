using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Fila;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("queue")]
[Tags("Fila de Atendimento")]
public class FilaController : ControllerBase
{
    private readonly IFilaUseCase _filaUseCase;

    public FilaController(IFilaUseCase filaUseCase)
    {
        _filaUseCase = filaUseCase;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // SIAH_Especificacao_API_v1.docx
    [HttpPost("validate-totem")]
    [ProducesResponseType(typeof(EmitirSenhaResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> ValidarTotem([FromBody] ValidarTotemRequest request)
    {
        var resultado = await _filaUseCase.ValidarTotemAsync(request);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("my-ticket")]
    [Authorize]
    [ProducesResponseType(typeof(SenhaAtivaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterSenhaAtiva()
    {
        try
        {
            var resultado = await _filaUseCase.ObterSenhaAtivaAsync(UserId);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("status/{ticketId:guid}")]
    [ProducesResponseType(typeof(StatusSenhaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterStatus(Guid ticketId)
    {
        try
        {
            var resultado = await _filaUseCase.ObterStatusAsync(ticketId);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPost("confirm-arrival")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmarPresenca([FromBody] ConfirmarPresencaRequest request)
    {
        await _filaUseCase.ConfirmarPresencaAsync(UserId, request.TicketId);
        return Ok(new { mensagem = "Presença confirmada com sucesso." });
    }
}
