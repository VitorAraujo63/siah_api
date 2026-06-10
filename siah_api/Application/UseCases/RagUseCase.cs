using SiahApi.Application.DTOs.Rag;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class RagUseCase : IRagUseCase
{
    private readonly IRagRepository _ragRepository;

    public RagUseCase(IRagRepository ragRepository)
    {
        _ragRepository = ragRepository;
    }

    public async Task<RagPatientDto?> ObterPacientePorCpfAsync(string cpf)
    {
        return await _ragRepository.ObterPacientePorCpfAsync(cpf);
    }

    public async Task<IEnumerable<RagConsultationDto>> ListarConsultasPorCpfAsync(string cpf)
    {
        return await _ragRepository.ListarConsultasPorCpfAsync(cpf);
    }
}
