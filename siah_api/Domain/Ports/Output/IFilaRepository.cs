using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IFilaRepository
{
    Task<SenhaAtendimento> EmitirSenhaAsync(SenhaAtendimento senha);
    Task<SenhaAtendimento?> ObterSenhaAtivaPorUsuarioAsync(string cpf);
    Task<SenhaAtendimento?> ObterPorIdAsync(long ticketId);
    Task<SenhaAtendimento> AtualizarStatusAsync(long ticketId, string novoStatus);
    Task<int> ObterProximaPosicaoAsync(string especialidade);
}
