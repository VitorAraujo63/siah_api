using SiahApi.Application.DTOs.Vacina;

namespace SiahApi.Domain.Ports.Input;

public interface IVacinaUseCase
{
    Task<IEnumerable<VacinaResponse>> ListarPorCpfAsync(string cpf, string? pesquisa = null);
    Task<VacinaResponse> CriarAsync(CriarVacinaRequest request);
    Task<VacinaResponse?> AtualizarAsync(Guid id, AtualizarVacinaRequest request);
    Task<bool> RemoverAsync(Guid id);
}
