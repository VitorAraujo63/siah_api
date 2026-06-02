namespace SiahApi.Domain.Entities;

public class Agendamento
{
    public Guid Id { get; set; }
    public Guid IdUsuario { get; set; }
    public Guid MedicoId { get; set; }
    public Guid EspecialidadeId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public Guid? PlanoId { get; set; }
    public string Status { get; set; } = "scheduled";
    public int? NumeroDaSenha { get; set; }
    public string? MotivoCancelamento { get; set; }
    public string? TipoCancelamento { get; set; }
    public bool ReembolsoElegivel { get; set; } = true;
    public DateTime? CanceladoEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
