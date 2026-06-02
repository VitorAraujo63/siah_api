using SiahApi.Domain.Entities;

namespace SiahApi.Domain.Ports.Output;

public interface IFilaRepository
{
    Task<SenhaAtendimento> EmitirSenhaAsync(SenhaAtendimento senha);
    Task<SenhaAtendimento?> ObterSenhaAtivaPorUsuarioAsync(Guid userId);
    Task<SenhaAtendimento?> ObterPorIdAsync(Guid ticketId);
    Task<SenhaAtendimento> AtualizarStatusAsync(Guid ticketId, string novoStatus);
    Task<int> ObterProximaPosicaoAsync(Guid departamentoId);
}
