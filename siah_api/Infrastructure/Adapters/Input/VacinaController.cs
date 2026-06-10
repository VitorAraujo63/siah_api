using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Vacina;
using SiahApi.Domain.Ports.Input;
using Swashbuckle.AspNetCore.Annotations;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("api/vacinas")]
[Tags("Vacinas")]
public class VacinaController : ControllerBase
{
    private readonly IVacinaUseCase _vacinaUseCase;

    public VacinaController(IVacinaUseCase vacinaUseCase)
    {
        _vacinaUseCase = vacinaUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VacinaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string cpf, [FromQuery] string? pesquisa = null)
    {
        var vacinas = await _vacinaUseCase.ListarPorCpfAsync(cpf, pesquisa);
        return Ok(vacinas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VacinaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Criar([FromBody] CriarVacinaRequest request)
    {
        try
        {
            var vacina = await _vacinaUseCase.CriarAsync(request);
            return CreatedAtAction(nameof(Listar), new { cpf = request.Cpf }, vacina);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VacinaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarVacinaRequest request)
    {
        var vacina = await _vacinaUseCase.AtualizarAsync(id, request);

        if (vacina == null)
            return NotFound(new { erro = "Vacina não encontrada." });

        return Ok(vacina);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id)
    {
        var removido = await _vacinaUseCase.RemoverAsync(id);

        if (!removido)
            return NotFound(new { erro = "Vacina não encontrada." });

        return Ok(new { mensagem = "Vacina removida com sucesso." });
    }
}
