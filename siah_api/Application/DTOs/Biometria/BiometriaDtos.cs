namespace SiahApi.Application.DTOs.Biometria;

public class CadastrarBiometriaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string TemplateBiometrico { get; set; } = string.Empty;
}

public class IdentificarBiometriaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string DigitalCapturada { get; set; } = string.Empty;
}

public class IdentificarBiometriaResponse
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public class ObterTemplateBiometriaResponse
{
    public string TemplateBiometrico { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
