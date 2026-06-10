using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Medico;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("doctors")]
[Tags("Medicos")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoUseCase _medicoUseCase;

    public MedicosController(IMedicoUseCase medicoUseCase)
    {
        _medicoUseCase = medicoUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MedicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] MedicoFiltros filtros)
    {
        var resultado = await _medicoUseCase.ListarAsync(filtros);
        return Ok(resultado);
    }

    [HttpGet("favorites")]
    [ProducesResponseType(typeof(IEnumerable<MedicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarFavoritos([FromQuery] string cpf)
    {
        var resultado = await _medicoUseCase.ListarFavoritosAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MedicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var resultado = await _medicoUseCase.ObterPorIdAsync(id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}/availability")]
    [ProducesResponseType(typeof(DisponibilidadeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterDisponibilidade(Guid id, [FromQuery] DisponibilidadeFiltros filtros)
    {
        var resultado = await _medicoUseCase.ObterDisponibilidadeAsync(id, filtros);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<AvaliacaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterAvaliacoes(Guid id)
    {
        var resultado = await _medicoUseCase.ObterAvaliacoesAsync(id);
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/favorite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Favoritar(Guid id, [FromQuery] string cpf)
    {
        await _medicoUseCase.FavoritarAsync(cpf, id);
        return Ok(new { mensagem = "Médico adicionado aos favoritos." });
    }

    [HttpDelete("{id:guid}/favorite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Desfavoritar(Guid id, [FromQuery] string cpf)
    {
        await _medicoUseCase.DesfavoritarAsync(cpf, id);
        return Ok(new { mensagem = "Médico removido dos favoritos." });
    }
}
