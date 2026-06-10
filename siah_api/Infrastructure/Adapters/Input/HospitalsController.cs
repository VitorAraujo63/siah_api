using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Hospital;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("Hospitals")]
[Tags("Hospitais")]
public class HospitalsController : ControllerBase
{
    private readonly IHospitalUseCase _hospitalUseCase;

    public HospitalsController(IHospitalUseCase hospitalUseCase)
    {
        _hospitalUseCase = hospitalUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HospitalResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var hospitais = await _hospitalUseCase.ListarAsync();
        return Ok(hospitais);
    }
}
