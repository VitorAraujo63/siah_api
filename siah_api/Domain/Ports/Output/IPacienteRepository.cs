using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IPacienteRepository
{
    Task<bool> ExistePorCpfAsync(string cpf);
    Task<Paciente> CriarAsync(Paciente paciente);
    Task<IEnumerable<(Guid Id, string Nome, string Cpf, float[] Embedding)>> ListarComEmbeddingAsync();
}
