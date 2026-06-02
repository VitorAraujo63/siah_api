using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface ITriagemRepository
{
    Task<Guid?> ObterIdUsuarioPorCpfAsync(string cpf);
    Task<Triagem> CriarAsync(Triagem triagem);
}
