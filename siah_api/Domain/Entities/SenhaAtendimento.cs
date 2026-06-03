namespace SiahApi.Domain.Entities;

public class SenhaAtendimento
{
    public long Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string NumeroSenha { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string? LocalChamada { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Status { get; set; } = "waiting";
}
