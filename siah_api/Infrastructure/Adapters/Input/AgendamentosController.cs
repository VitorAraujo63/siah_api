using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Agendamento;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("appointments")]
[Authorize]
[Tags("Agendamentos")]
public class AgendamentosController : ControllerBase
{
    private readonly IAgendamentoUseCase _agendamentoUseCase;

    public AgendamentosController(IAgendamentoUseCase agendamentoUseCase)
    {
        _agendamentoUseCase = agendamentoUseCase;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // SIAH_Especificacao_API_v1.docx
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] AgendamentoFiltros filtros)
    {
        var resultado = await _agendamentoUseCase.ListarAsync(UserId, filtros);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarProximos()
    {
        var resultado = await _agendamentoUseCase.ListarProximosAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var resultado = await _agendamentoUseCase.ObterPorIdAsync(UserId, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Agendar([FromBody] AgendarRequest request)
    {
        try
        {
            var resultado = await _agendamentoUseCase.AgendarAsync(UserId, request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { sucesso = false, error = "SLOT_UNAVAILABLE", mensagem = "Este horário não está mais disponível." });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpPatch("{id:guid}/reschedule")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reagendar(Guid id, [FromBody] ReagendarRequest request)
    {
        try
        {
            var resultado = await _agendamentoUseCase.ReagendarAsync(UserId, id, request);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpDelete("{id:guid}/cancel")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarRequest request)
    {
        try
        {
            var resultado = await _agendamentoUseCase.CancelarAsync(UserId, id, request);
            return Ok(new { sucesso = true, mensagem = "Consulta cancelada com sucesso.", data = resultado });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
