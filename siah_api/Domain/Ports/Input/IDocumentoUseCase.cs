using SiahApi.Application.DTOs.Documento;

namespace SiahApi.Domain.Ports.Input;

public interface IDocumentoUseCase
{
    Task<IEnumerable<DocumentoResponse>> ListarAtestadosAsync(Guid userId);
    Task<byte[]> ObterAtestadoPdfAsync(Guid userId, Guid documentoId);
    Task<IEnumerable<DocumentoResponse>> ListarExamesAsync(Guid userId);
    Task<DocumentoResponse> ObterResultadoExameAsync(Guid userId, Guid exameId);
    Task<IEnumerable<DocumentoResponse>> ListarReceitasAsync(Guid userId);
    Task<IEnumerable<DocumentoResponse>> ListarReceitasAtivasAsync(Guid userId);
    Task<IEnumerable<DocumentoResponse>> BuscarAsync(Guid userId, string termo);
}
