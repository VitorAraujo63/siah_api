using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Paciente;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("api/pacientes")]
[Tags("Pacientes")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteUseCase _pacienteUseCase;

    public PacientesController(IPacienteUseCase pacienteUseCase)
    {
        _pacienteUseCase = pacienteUseCase;
    }

    // Documentação de Integração_ SIAH API C#.pdf
    [HttpPost("cadastrar")]
    [ProducesResponseType(typeof(CadastrarPacienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPacienteRequest request)
    {
        try
        {
            var resultado = await _pacienteUseCase.CadastrarAsync(request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // Documentação de Integração_ SIAH API C#.pdf
    [HttpPost("reconhecer")]
    [ProducesResponseType(typeof(ReconhecerPacienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Reconhecer([FromBody] ReconhecerPacienteRequest request)
    {
        var resultado = await _pacienteUseCase.ReconhecerAsync(request);

        if (!resultado.Sucesso)
            return Unauthorized(new { mensagem = "Rosto não reconhecido na base de dados." });

        return Ok(resultado);
    }
}
