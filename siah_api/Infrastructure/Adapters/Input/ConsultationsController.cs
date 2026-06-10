using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Consulta;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("Consultations")]
[Tags("Consultas")]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultaUseCase _consultaUseCase;

    public ConsultationsController(IConsultaUseCase consultaUseCase)
    {
        _consultaUseCase = consultaUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsultaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string cpf, [FromQuery] string? pesquisa = null)
    {
        var consultas = await _consultaUseCase.ListarPorCpfAsync(cpf, pesquisa);
        return Ok(consultas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CriarConsultaRequest request)
    {
        var consulta = await _consultaUseCase.CriarAsync(request);
        return StatusCode(StatusCodes.Status201Created, consulta);
    }
}
