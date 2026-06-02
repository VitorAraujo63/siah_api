using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Medico;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("search")]
[Authorize]
[Tags("Busca")]
public class BuscaController : ControllerBase
{
    private readonly IMedicoUseCase _medicoUseCase;

    public BuscaController(IMedicoUseCase medicoUseCase)
    {
        _medicoUseCase = medicoUseCase;
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("doctors")]
    [ProducesResponseType(typeof(IEnumerable<MedicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BuscarMedicos([FromQuery] string q)
    {
        var resultado = await _medicoUseCase.BuscarAsync(q);
        return Ok(resultado);
    }
}
