using SiahApi.Application.DTOs.Paciente;
using SiahApi.Application.Services;
using SiahApi.Domain.Entities;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;

namespace SiahApi.Application.UseCases;

public class PacienteUseCase : IPacienteUseCase
{
    private const double ThresholdCosseno = 0.60;

    private readonly IPacienteRepository _pacienteRepository;
    private readonly IFaceEmbeddingService _faceEmbeddingService;

    public PacienteUseCase(IPacienteRepository pacienteRepository, IFaceEmbeddingService faceEmbeddingService)
    {
        _pacienteRepository = pacienteRepository;
        _faceEmbeddingService = faceEmbeddingService;
    }

    public async Task<CadastrarPacienteResponse> CadastrarAsync(CadastrarPacienteRequest request)
    {
        var cpfExiste = await _pacienteRepository.ExistePorCpfAsync(request.Cpf);
        if (cpfExiste)
            throw new InvalidOperationException("CPF já cadastrado na base.");

        var paciente = new Paciente
        {
            Nome = request.Nome,
            Cpf = request.Cpf,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
            Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone,
            DataNascimento = request.DataNascimento,
            Genero = request.Genero,
            TipoSanguineo = request.TipoSanguineo,
            HospitalVinculado = request.HospitalVinculado,
            Rg = request.Rg,
            CartaoSus = request.CartaoSus,
            Cnh = request.Cnh,
            Cep = request.Cep,
            Rua = request.Rua,
            Numero = request.Numero,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            PossuiPlanoSaude = request.PossuiPlanoSaude,
            NomePlano = string.IsNullOrWhiteSpace(request.NomePlano) ? null : request.NomePlano,
            NumeroCarteirinha = string.IsNullOrWhiteSpace(request.NumeroCarteirinha) ? null : request.NumeroCarteirinha,
            ValidadeCarteirinha = string.IsNullOrWhiteSpace(request.ValidadeCarteirinha) ? null : request.ValidadeCarteirinha,
            NomeResponsavel = string.IsNullOrWhiteSpace(request.NomeResponsavel) ? null : request.NomeResponsavel,
            Parentesco = string.IsNullOrWhiteSpace(request.Parentesco) ? null : request.Parentesco,
            TelefoneResponsavel = string.IsNullOrWhiteSpace(request.TelefoneResponsavel) ? null : request.TelefoneResponsavel,
            Images = request.Images,
            Embedding = request.Embedding,
            EmbeddingPath = request.EmbeddingPath
        };

        var criado = await _pacienteRepository.CriarAsync(paciente);

        return new CadastrarPacienteResponse
        {
            Id = criado.Id,
            Nome = criado.Nome,
            Cpf = criado.Cpf,
            EmbeddingPath = criado.EmbeddingPath,
            Images = criado.Images
        };
    }

    public async Task<ReconhecerPacienteResponse> ReconhecerAsync(ReconhecerPacienteRequest request)
    {
        if (request.Images == null || request.Images.Count == 0)
            return new ReconhecerPacienteResponse { Sucesso = false };

        var embeddingConsulta = await _faceEmbeddingService.GerarEmbeddingAsync(request.Images[0]);

        var pacientesComEmbedding = await _pacienteRepository.ListarComEmbeddingAsync();

        double menorDistancia = double.MaxValue;
        (Guid Id, string Nome, string Cpf, float[] Embedding) melhorMatch = (Guid.Empty, string.Empty, string.Empty, Array.Empty<float>());

        foreach (var paciente in pacientesComEmbedding)
        {
            var distancia = CalcularDistanciaCosseno(embeddingConsulta, paciente.Embedding);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                melhorMatch = paciente;
            }
        }

        if (menorDistancia >= ThresholdCosseno)
            return new ReconhecerPacienteResponse { Sucesso = false };

        return new ReconhecerPacienteResponse
        {
            Sucesso = true,
            Paciente = new PacienteReconhecidoDto
            {
                Id = melhorMatch.Id,
                Nome = melhorMatch.Nome,
                Cpf = melhorMatch.Cpf,
                Distancia = menorDistancia
            }
        };
    }

    private static double CalcularDistanciaCosseno(float[] a, float[] b)
    {
        if (a.Length != b.Length || a.Length == 0) return 1.0;

        double produtoInterno = 0;
        double normaA = 0;
        double normaB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            produtoInterno += a[i] * b[i];
            normaA += a[i] * a[i];
            normaB += b[i] * b[i];
        }

        if (normaA == 0 || normaB == 0) return 1.0;

        return 1.0 - (produtoInterno / (Math.Sqrt(normaA) * Math.Sqrt(normaB)));
    }
}
