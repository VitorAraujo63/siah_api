using SiahApi.Application.DTOs.Especialidade;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class EspecialidadeUseCase : IEspecialidadeUseCase
{
    private readonly IEspecialidadeRepository _especialidadeRepository;

    public EspecialidadeUseCase(IEspecialidadeRepository especialidadeRepository)
    {
        _especialidadeRepository = especialidadeRepository;
    }

    public async Task<IEnumerable<EspecialidadeResponse>> ListarAsync()
    {
        var especialidades = await _especialidadeRepository.ListarAsync();
        return especialidades.Select(e => new EspecialidadeResponse
        {
            Id = e.Id,
            Nome = e.Nome,
            Descricao = e.Descricao
        });
    }

    public async Task<IEnumerable<MedicoSummaryResponse>> ListarMedicosPorEspecialidadeAsync(Guid especialidadeId)
    {
        var medicos = await _especialidadeRepository.ListarMedicosPorEspecialidadeAsync(especialidadeId);
        return medicos.Select(m => new MedicoSummaryResponse
        {
            Id = m.Id,
            Nome = m.Nome,
            FotoUrl = m.FotoUrl,
            Rating = m.Rating
        });
    }
}
