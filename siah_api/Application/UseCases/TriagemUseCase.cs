using SiahApi.Application.DTOs.Triagem;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class TriagemUseCase : ITriagemUseCase
{
    private readonly ITriagemRepository _triagemRepository;

    public TriagemUseCase(ITriagemRepository triagemRepository)
    {
        _triagemRepository = triagemRepository;
    }

    public async Task<RegistrarTriagemResponse> RegistrarAsync(RegistrarTriagemRequest request)
    {
        var idUsuario = await _triagemRepository.ObterIdUsuarioPorCpfAsync(request.CpfPaciente);
        if (idUsuario is null)
            throw new KeyNotFoundException("Paciente não encontrado para o CPF informado.");

        var triagem = new Triagem
        {
            IdUsuario = idUsuario.Value,
            QueixaPrincipal = request.QueixaPrincipal,
            Peso = string.IsNullOrWhiteSpace(request.Peso) ? null : request.Peso,
            Altura = string.IsNullOrWhiteSpace(request.Altura) ? null : request.Altura,
            Temperatura = string.IsNullOrWhiteSpace(request.Temperatura) ? null : request.Temperatura,
            PressaoArterial = string.IsNullOrWhiteSpace(request.PressaoArterial) ? null : request.PressaoArterial,
            FrequenciaCardiaca = string.IsNullOrWhiteSpace(request.FrequenciaCardiaca) ? null : request.FrequenciaCardiaca
        };

        var criada = await _triagemRepository.CriarAsync(triagem);

        return new RegistrarTriagemResponse
        {
            Status = "sucesso",
            Data = new TriagemDataDto
            {
                Id = criada.Id,
                IdUsuario = criada.IdUsuario,
                QueixaPrincipal = criada.QueixaPrincipal,
                Peso = criada.Peso,
                Altura = criada.Altura,
                Temperatura = criada.Temperatura,
                PressaoArterial = criada.PressaoArterial,
                FrequenciaCardiaca = criada.FrequenciaCardiaca
            }
        };
    }
}
