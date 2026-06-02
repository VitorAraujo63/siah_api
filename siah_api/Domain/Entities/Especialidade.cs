namespace SiahApi.Domain.Entities;

public class Especialidade
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
