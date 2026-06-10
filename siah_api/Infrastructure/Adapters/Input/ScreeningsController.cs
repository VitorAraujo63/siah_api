using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Screening;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("Screenings")]
[Tags("Triagens")]
public class ScreeningsController : ControllerBase
{
    private readonly IScreeningUseCase _screeningUseCase;

    public ScreeningsController(IScreeningUseCase screeningUseCase)
    {
        _screeningUseCase = screeningUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScreeningResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string cpf, [FromQuery] string? pesquisa = null)
    {
        var triagens = await _screeningUseCase.ListarPorCpfAsync(cpf, pesquisa);
        return Ok(triagens);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScreeningResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CriarScreeningRequest request)
    {
        try
        {
            var triagem = await _screeningUseCase.CriarAsync(request);
            return StatusCode(StatusCodes.Status201Created, triagem);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
