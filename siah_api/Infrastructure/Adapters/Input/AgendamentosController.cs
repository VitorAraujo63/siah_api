using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Agendamento;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("appointments")]
[Tags("Agendamentos")]
public class AgendamentosController : ControllerBase
{
    private readonly IAgendamentoUseCase _agendamentoUseCase;

    public AgendamentosController(IAgendamentoUseCase agendamentoUseCase)
    {
        _agendamentoUseCase = agendamentoUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] AgendamentoFiltros filtros, [FromQuery] string cpf)
    {
        var resultado = await _agendamentoUseCase.ListarAsync(cpf, filtros);
        return Ok(resultado);
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarProximos([FromQuery] string cpf)
    {
        var resultado = await _agendamentoUseCase.ListarProximosAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _agendamentoUseCase.ObterPorIdAsync(cpf, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Agendar([FromBody] AgendarRequest request, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _agendamentoUseCase.AgendarAsync(cpf, request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { sucesso = false, error = "SLOT_UNAVAILABLE", mensagem = "Este horário não está mais disponível." });
        }
    }

    [HttpPatch("{id:guid}/reschedule")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reagendar(Guid id, [FromBody] ReagendarRequest request, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _agendamentoUseCase.ReagendarAsync(cpf, id, request);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/cancel")]
    [ProducesResponseType(typeof(AgendamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarRequest request, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _agendamentoUseCase.CancelarAsync(cpf, id, request);
            return Ok(new { sucesso = true, mensagem = "Consulta cancelada com sucesso.", data = resultado });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
