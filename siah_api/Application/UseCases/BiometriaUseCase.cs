using SiahApi.Application.DTOs.Biometria;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class BiometriaUseCase : IBiometriaUseCase
{
    private readonly IBiometriaRepository _biometriaRepository;

    public BiometriaUseCase(IBiometriaRepository biometriaRepository)
    {
        _biometriaRepository = biometriaRepository;
    }

    public async Task CadastrarAsync(CadastrarBiometriaRequest request)
    {
        var existe = await _biometriaRepository.ExistePorCpfAsync(request.Cpf);
        if (!existe)
            throw new KeyNotFoundException("CPF não encontrado na base.");

        var templateBytes = Convert.FromBase64String(request.TemplateBiometrico);
        var atualizado = await _biometriaRepository.AtualizarTemplateBiometricoAsync(request.Cpf, templateBytes);

        if (!atualizado)
            throw new InvalidOperationException("Falha ao atualizar o template biométrico.");
    }

    public async Task<IdentificarBiometriaResponse> IdentificarAsync(IdentificarBiometriaRequest request)
    {
        var digitalBytes = Convert.FromBase64String(request.DigitalCapturada);
        var pacientes = await _biometriaRepository.ListarComTemplateBiometricoAsync();

        foreach (var paciente in pacientes)
        {
            if (VerificarCorrespondencia(digitalBytes, paciente.Template))
            {
                return new IdentificarBiometriaResponse
                {
                    Cpf = paciente.Cpf,
                    Nome = paciente.Nome
                };
            }
        }

        throw new UnauthorizedAccessException("Digital não reconhecida na base de dados.");
    }

    private static bool VerificarCorrespondencia(byte[] digitalConsulta, byte[] templateBanco)
    {
        return false;
    }
}
