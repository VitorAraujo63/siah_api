namespace SiahApi.Application.DTOs.PacientDetail;

public class PacientDetailResponse
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string? EmbeddingPath { get; set; }
    public List<string>? Images { get; set; }
    public object? Embeddings { get; set; }
    public DateTime? CriadoEm { get; set; }
    public string? Rg { get; set; }
    public string? DataNascimento { get; set; }
    public string? Genero { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Naturalidade { get; set; }
    public string? Telefone { get; set; }
    public string? TelefoneSecundario { get; set; }
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? HospitalVinculado { get; set; }
    public string? MedicoResponsavel { get; set; }
    public string? TipoSanguineo { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Altura { get; set; }
    public decimal? Imc { get; set; }
    public string? PressaoArterial { get; set; }
    public string? FrequenciaCardiaca { get; set; }
    public string? Alergias { get; set; }
    public string? CondicoesCronicas { get; set; }
    public string? HistoricoFamiliar { get; set; }
    public bool? PossuiPlanoSaude { get; set; }
    public string? NomePlano { get; set; }
    public string? NumeroCarteirinha { get; set; }
    public string? ValidadeCarteirinha { get; set; }
    public string? NomeResponsavel { get; set; }
    public string? Parentesco { get; set; }
    public string? TelefoneResponsavel { get; set; }
}
