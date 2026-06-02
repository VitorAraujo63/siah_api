using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Especialidade;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("specialties")]
[Authorize]
[Tags("Especialidades")]
public class EspecialidadesController : ControllerBase
{
    private readonly IEspecialidadeUseCase _especialidadeUseCase;

    public EspecialidadesController(IEspecialidadeUseCase especialidadeUseCase)
    {
        _especialidadeUseCase = especialidadeUseCase;
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EspecialidadeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var resultado = await _especialidadeUseCase.ListarAsync();
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("{id:guid}/doctors")]
    [ProducesResponseType(typeof(IEnumerable<MedicoSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarMedicos(Guid id)
    {
        var resultado = await _especialidadeUseCase.ListarMedicosPorEspecialidadeAsync(id);
        return Ok(resultado);
    }
}
