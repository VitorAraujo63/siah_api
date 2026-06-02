namespace SiahApi.Domain.Entities;

public class SenhaAtendimento
{
    public Guid Id { get; set; }
    public Guid IdPaciente { get; set; }
    public string TotemId { get; set; } = string.Empty;
    public string TipoServico { get; set; } = string.Empty;
    public Guid DepartamentoId { get; set; }
    public string NumeroSenha { get; set; } = string.Empty;
    public int PosicaoNaFila { get; set; }
    public int TempoEsperaMinutos { get; set; }
    public string Status { get; set; } = "waiting";
    public DateTime EmitidaEm { get; set; } = DateTime.UtcNow;
    public DateTime? ChamadaEm { get; set; }
    public DateTime? ConcluidaEm { get; set; }
}
