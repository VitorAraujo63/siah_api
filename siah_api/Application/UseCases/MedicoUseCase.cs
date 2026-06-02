using SiahApi.Application.DTOs.Medico;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class MedicoUseCase : IMedicoUseCase
{
    private readonly IMedicoRepository _medicoRepository;

    public MedicoUseCase(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<IEnumerable<MedicoResponse>> ListarAsync(MedicoFiltros filtros)
    {
        var medicos = await _medicoRepository.ListarAsync(filtros);
        return medicos.Select(m => new MedicoResponse
        {
            Id = m.Id,
            Nome = m.Nome,
            Especialidade = m.Especialidade,
            FotoUrl = m.FotoUrl,
            Rating = m.Rating,
            DisponivelHoje = m.DisponivelHoje
        });
    }

    public async Task<IEnumerable<MedicoResponse>> ListarFavoritosAsync(Guid userId)
    {
        var medicos = await _medicoRepository.ListarFavoritosPorUsuarioAsync(userId);
        return medicos.Select(m => new MedicoResponse
        {
            Id = m.Id,
            Nome = m.Nome,
            Especialidade = m.Especialidade,
            FotoUrl = m.FotoUrl,
            Rating = m.Rating,
            DisponivelHoje = m.DisponivelHoje
        });
    }

    public async Task<MedicoResponse> ObterPorIdAsync(Guid medicoId)
    {
        var medico = await _medicoRepository.ObterPorIdAsync(medicoId)
            ?? throw new KeyNotFoundException("Médico não encontrado.");

        return new MedicoResponse
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Especialidade = medico.Especialidade,
            FotoUrl = medico.FotoUrl,
            Rating = medico.Rating,
            DisponivelHoje = medico.DisponivelHoje
        };
    }

    public async Task<DisponibilidadeResponse> ObterDisponibilidadeAsync(Guid medicoId, DisponibilidadeFiltros filtros)
    {
        var slots = await _medicoRepository.ObterDisponibilidadeAsync(medicoId, filtros.DateStart, filtros.DateEnd, filtros.SpecialtyId);

        return new DisponibilidadeResponse
        {
            DoctorId = medicoId,
            Slots = slots.Select(s => new SlotDto
            {
                Date = s.Data,
                Time = s.Horario,
                Available = s.Disponivel
            }).ToList()
        };
    }

    public async Task<IEnumerable<AvaliacaoResponse>> ObterAvaliacoesAsync(Guid medicoId)
    {
        var avaliacoes = await _medicoRepository.ObterAvaliacoesAsync(medicoId);
        return avaliacoes.Select(a => new AvaliacaoResponse
        {
            Avaliador = a.Avaliador,
            Comentario = a.Comentario,
            Nota = a.Nota,
            Data = a.Data
        });
    }

    public Task FavoritarAsync(Guid userId, Guid medicoId)
        => _medicoRepository.FavoritarAsync(userId, medicoId);

    public Task DesfavoritarAsync(Guid userId, Guid medicoId)
        => _medicoRepository.DesfavoritarAsync(userId, medicoId);

    public async Task<IEnumerable<MedicoResponse>> BuscarAsync(string termo)
    {
        var medicos = await _medicoRepository.BuscarAsync(termo);
        return medicos.Select(m => new MedicoResponse
        {
            Id = m.Id,
            Nome = m.Nome,
            Especialidade = m.Especialidade,
            FotoUrl = m.FotoUrl,
            Rating = m.Rating,
            DisponivelHoje = m.DisponivelHoje
        });
    }
}
