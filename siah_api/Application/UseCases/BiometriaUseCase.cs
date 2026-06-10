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
        var dadosPaciente = await _biometriaRepository.ObterTemplatePorCpfAsync(request.Cpf);

        if (dadosPaciente == null)
        {
            throw new UnauthorizedAccessException("CPF não encontrado ou sem biometria cadastrada.");
        }

        if (VerificarCorrespondencia(digitalBytes, dadosPaciente.Value.Template))
        {
            return new IdentificarBiometriaResponse
            {
                Cpf = request.Cpf,
                Nome = dadosPaciente.Value.Nome
            };
        }

        throw new UnauthorizedAccessException("Digital não reconhecida para este CPF.");
    }

    private static bool VerificarCorrespondencia(byte[] digitalConsulta, byte[] templateBanco)
    {
        return false;
    }

    public async Task<ObterTemplateBiometriaResponse?> ObterTemplatePorCpfAsync(string cpf)
    {
        var dados = await _biometriaRepository.ObterTemplatePorCpfAsync(cpf);

        if (dados == null)
            return null;

        return new ObterTemplateBiometriaResponse
        {
            TemplateBiometrico = Convert.ToBase64String(dados.Value.Template),
            Nome = dados.Value.Nome
        };
    }
}
