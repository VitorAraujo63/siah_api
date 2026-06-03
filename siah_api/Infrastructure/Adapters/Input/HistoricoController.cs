using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Historico;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

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

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("appointments")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] HistoricoFiltros filtros, [FromQuery] Guid userId)
    {
        var resultado = await _historicoUseCase.ListarAsync(userId, filtros);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("appointments/{id:guid}")]
    [ProducesResponseType(typeof(HistoricoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, [FromQuery] Guid userId)
    {
        try
        {
            var resultado = await _historicoUseCase.ObterPorIdAsync(userId, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarRecentes([FromQuery] Guid userId)
    {
        var resultado = await _historicoUseCase.ListarRecentesAsync(userId);
        return Ok(resultado);
    }
}
