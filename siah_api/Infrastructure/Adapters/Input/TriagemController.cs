using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Triagem;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("api/triagem")]
[Tags("Triagem")]
public class TriagemController : ControllerBase
{
    private readonly ITriagemUseCase _triagemUseCase;

    public TriagemController(ITriagemUseCase triagemUseCase)
    {
        _triagemUseCase = triagemUseCase;
    }

    // Documentação de Integração_ SIAH API C#.pdf
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(RegistrarTriagemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarTriagemRequest request)
    {
        try
        {
            var resultado = await _triagemUseCase.RegistrarAsync(request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
