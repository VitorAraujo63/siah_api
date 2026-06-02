using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IDocumentoRepository
{
    Task<IEnumerable<Documento>> ListarPorTipoAsync(Guid userId, string tipo);
    Task<Documento?> ObterPorIdAsync(Guid documentoId);
    Task<byte[]?> ObterPdfAsync(Guid documentoId);
    Task<IEnumerable<Documento>> BuscarAsync(Guid userId, string termo);
}
