using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Documento;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

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

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("certificates")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAtestados([FromQuery] Guid userId)
    {
        var resultado = await _documentoUseCase.ListarAtestadosAsync(userId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("certificates/{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAtestadoPdf(Guid id, [FromQuery] Guid userId)
    {
        try
        {
            var pdf = await _documentoUseCase.ObterAtestadoPdfAsync(userId, id);
            return File(pdf, "application/pdf", $"atestado-{id}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("exams")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarExames([FromQuery] Guid userId)
    {
        var resultado = await _documentoUseCase.ListarExamesAsync(userId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("exams/{id:guid}/result")]
    [ProducesResponseType(typeof(DocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterResultadoExame(Guid id, [FromQuery] Guid userId)
    {
        try
        {
            var resultado = await _documentoUseCase.ObterResultadoExameAsync(userId, id);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("prescriptions")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReceitas([FromQuery] Guid userId)
    {
        var resultado = await _documentoUseCase.ListarReceitasAsync(userId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("prescriptions/active")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReceitasAtivas([FromQuery] Guid userId)
    {
        var resultado = await _documentoUseCase.ListarReceitasAtivasAsync(userId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Buscar([FromQuery] string q, [FromQuery] Guid userId)
    {
        var resultado = await _documentoUseCase.BuscarAsync(userId, q);
        return Ok(resultado);
    }
}
