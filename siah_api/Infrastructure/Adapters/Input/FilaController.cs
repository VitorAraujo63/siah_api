using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Fila;
using SiahApi.Domain.Ports.Input;

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

    [HttpPost("validate-totem")]
    [ProducesResponseType(typeof(EmitirSenhaResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> ValidarTotem([FromBody] ValidarTotemRequest request)
    {
        var resultado = await _filaUseCase.ValidarTotemAsync(request);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }

    [HttpGet("my-ticket")]
    [ProducesResponseType(typeof(SenhaAtivaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterSenhaAtiva([FromQuery] string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new { erro = "O CPF é obrigatório." });

        try
        {
            var resultado = await _filaUseCase.ObterSenhaAtivaAsync(cpf);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("status/{ticketId:long}")]
    [ProducesResponseType(typeof(StatusSenhaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterStatus(long ticketId)
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

    [HttpPost("confirm-arrival")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmarPresenca([FromBody] ConfirmarPresencaRequest request)
    {
        await _filaUseCase.ConfirmarPresencaAsync(request.TicketId);
        return Ok(new { mensagem = "Presença confirmada com sucesso." });
    }
}
