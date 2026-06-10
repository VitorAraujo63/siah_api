using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Exame;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("Exams")]
[Tags("Exames")]
public class ExamsController : ControllerBase
{
    private readonly IExameUseCase _exameUseCase;

    public ExamsController(IExameUseCase exameUseCase)
    {
        _exameUseCase = exameUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExameResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string cpf, [FromQuery] string? pesquisa = null)
    {
        var exames = await _exameUseCase.ListarPorCpfAsync(cpf, pesquisa);
        return Ok(exames);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CriarExameRequest request)
    {
        var exame = await _exameUseCase.CriarAsync(request);
        return StatusCode(StatusCodes.Status201Created, exame);
    }
}
