using SiahApi.Application.DTOs.Documento;

namespace SiahApi.Domain.Ports.Input;

public interface IDocumentoUseCase
{
    Task<IEnumerable<DocumentoResponse>> ListarAtestadosAsync(string cpf);
    Task<byte[]> ObterAtestadoPdfAsync(string cpf, Guid documentoId);
    Task<IEnumerable<DocumentoResponse>> ListarExamesAsync(string cpf);
    Task<DocumentoResponse> ObterResultadoExameAsync(string cpf, Guid exameId);
    Task<IEnumerable<DocumentoResponse>> ListarReceitasAsync(string cpf);
    Task<IEnumerable<DocumentoResponse>> ListarReceitasAtivasAsync(string cpf);
    Task<IEnumerable<DocumentoResponse>> BuscarAsync(string cpf, string termo);
}
