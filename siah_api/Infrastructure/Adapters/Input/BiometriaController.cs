using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Biometria;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("api/biometria")]
[Tags("Biometria")]
public class BiometriaController : ControllerBase
{
    private readonly IBiometriaUseCase _biometriaUseCase;

    public BiometriaController(IBiometriaUseCase biometriaUseCase)
    {
        _biometriaUseCase = biometriaUseCase;
    }

    // documentacao.pdf
    [HttpPost("cadastrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarBiometriaRequest request)
    {
        try
        {
            await _biometriaUseCase.CadastrarAsync(request);
            return Ok(new { mensagem = "Template biométrico cadastrado com sucesso." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // documentacao.pdf
    [HttpPost("identificar")]
    [ProducesResponseType(typeof(IdentificarBiometriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Identificar([FromBody] IdentificarBiometriaRequest request)
    {
        try
        {
            var resultado = await _biometriaUseCase.IdentificarAsync(request);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { erro = ex.Message });
        }
    }
}
