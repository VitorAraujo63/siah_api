namespace SiahApi.Domain.Ports.Output;

public interface IBiometriaRepository
{
    Task<bool> ExistePorCpfAsync(string cpf);
    Task<bool> AtualizarTemplateBiometricoAsync(string cpf, byte[] template);
    Task<IEnumerable<(string Cpf, string Nome, byte[] Template)>> ListarComTemplateBiometricoAsync();
}
