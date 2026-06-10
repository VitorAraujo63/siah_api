using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.PacientDetail;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("Pacient")]
[Tags("Paciente Detalhes")]
public class PacientController : ControllerBase
{
    private readonly IPacientDetailUseCase _pacientDetailUseCase;

    public PacientController(IPacientDetailUseCase pacientDetailUseCase)
    {
        _pacientDetailUseCase = pacientDetailUseCase;
    }

    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(PacientDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorCpf(string cpf)
    {
        var paciente = await _pacientDetailUseCase.ObterPorCpfAsync(cpf);
        
        if (paciente == null)
            return NotFound(new { erro = "Paciente não encontrado." });
            
        return Ok(paciente);
    }
}
