using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Documento;
using SiahApi.Domain.Ports.Input;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("documents")]
[Tags("Documentos Clinicos")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoUseCase _documentoUseCase;

    public DocumentosController(IDocumentoUseCase documentoUseCase)
    {
        _documentoUseCase = documentoUseCase;
    }

    [HttpGet("certificates")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAtestados([FromQuery] string cpf)
    {
        var resultado = await _documentoUseCase.ListarAtestadosAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("certificates/{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAtestadoPdf(Guid id, [FromQuery] string cpf)
    {
        try
        {
            var pdf = await _documentoUseCase.ObterAtestadoPdfAsync(cpf, id);
            return File(pdf, "application/pdf", $"atestado-{id}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("exams")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarExames([FromQuery] string cpf)
    {
        var resultado = await _documentoUseCase.ListarExamesAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("exams/{id:guid}/result")]
    [ProducesResponseType(typeof(DocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterResultadoExame(Guid id, [FromQuery] string cpf)
    {
        try
        {
            var resultado = await _documentoUseCase.ObterResultadoExameAsync(cpf, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("prescriptions")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReceitas([FromQuery] string cpf)
    {
        var resultado = await _documentoUseCase.ListarReceitasAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("prescriptions/active")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReceitasAtivas([FromQuery] string cpf)
    {
        var resultado = await _documentoUseCase.ListarReceitasAtivasAsync(cpf);
        return Ok(resultado);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Buscar([FromQuery] string q, [FromQuery] string cpf)
    {
        var resultado = await _documentoUseCase.BuscarAsync(cpf, q);
        return Ok(resultado);
    }
}
