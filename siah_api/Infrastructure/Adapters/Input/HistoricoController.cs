using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Historico;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("history")]
[Tags("Historico")]
public class HistoricoController : ControllerBase
{
    private readonly IHistoricoUseCase _historicoUseCase;

    public HistoricoController(IHistoricoUseCase historicoUseCase)
    {
        _historicoUseCase = historicoUseCase;
    }

    [HttpGet("appointments")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] HistoricoFiltros filtros, [FromQuery] string cpf)
    {
        var resultado = await _historicoUseCase.ListarAsync(cpf, filtros);
        return Ok(resultado);
    }

    [HttpGet("appointments/{id:guid}")]
    [ProducesResponseType(typeof(HistoricoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _historicoUseCase.ObterPorIdAsync(cpf, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarRecentes([FromQuery] string cpf)
    {
        var resultado = await _historicoUseCase.ListarRecentesAsync(cpf);
        return Ok(resultado);
    }
}
