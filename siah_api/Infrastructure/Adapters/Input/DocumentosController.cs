using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiahApi.Application.DTOs.Documento;
using SiahApi.Domain.Ports.Input;
using System.Security.Claims;

namespace SiahApi.Infrastructure.Adapters.Input;

[ApiController]
[Route("documents")]
[Authorize]
[Tags("Documentos Clinicos")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoUseCase _documentoUseCase;

    public DocumentosController(IDocumentoUseCase documentoUseCase)
    {
        _documentoUseCase = documentoUseCase;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("certificates")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAtestados()
    {
        var resultado = await _documentoUseCase.ListarAtestadosAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("certificates/{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAtestadoPdf(Guid id)
    {
        try
        {
            var pdf = await _documentoUseCase.ObterAtestadoPdfAsync(UserId, id);
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
    public async Task<IActionResult> ListarExames()
    {
        var resultado = await _documentoUseCase.ListarExamesAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("exams/{id:guid}/result")]
    [ProducesResponseType(typeof(DocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterResultadoExame(Guid id)
    {
        try
        {
            var resultado = await _documentoUseCase.ObterResultadoExameAsync(UserId, id);
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
    public async Task<IActionResult> ListarReceitas()
    {
        var resultado = await _documentoUseCase.ListarReceitasAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("prescriptions/active")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReceitasAtivas()
    {
        var resultado = await _documentoUseCase.ListarReceitasAtivasAsync(UserId);
        return Ok(resultado);
    }

    // SIAH_Especificacao_API_v1.docx
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        var resultado = await _documentoUseCase.BuscarAsync(UserId, q);
        return Ok(resultado);
    }
}
