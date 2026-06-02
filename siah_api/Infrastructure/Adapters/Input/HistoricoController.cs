using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Historico;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("history")]
[Authorize]
[Tags("Historico")]
public class HistoricoController : ControllerBase
{
    private readonly IHistoricoUseCase _historicoUseCase;

    public HistoricoController(IHistoricoUseCase historicoUseCase)
    {
        _historicoUseCase = historicoUseCase;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("appointments")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] HistoricoFiltros filtros)
    {
        var resultado = await _historicoUseCase.ListarAsync(UserId, filtros);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("appointments/{id:guid}")]
    [ProducesResponseType(typeof(HistoricoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var resultado = await _historicoUseCase.ObterPorIdAsync(UserId, id);
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
    public async Task<IActionResult> ListarRecentes()
    {
        var resultado = await _historicoUseCase.ListarRecentesAsync(UserId);
        return Ok(resultado);
    }
}
