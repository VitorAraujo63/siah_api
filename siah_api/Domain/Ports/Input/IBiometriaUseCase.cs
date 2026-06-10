using SiahApi.Application.DTOs.Biometria;

namespace SiahApi.Domain.Ports.Input;

public interface IBiometriaUseCase
{
    Task CadastrarAsync(CadastrarBiometriaRequest request);
    Task<IdentificarBiometriaResponse> IdentificarAsync(IdentificarBiometriaRequest request);
    Task<ObterTemplateBiometriaResponse?> ObterTemplatePorCpfAsync(string cpf);
}
