using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SiahApi.Application.Services;

namespace SiahApi.Infrastructure.Services;

/// <summary>
/// Implementação real do gerador de embeddings faciais. Delega o cálculo do vetor
/// VGG-Face (512-dim) ao microserviço Python (DeepFace) configurado em
/// "FaceEmbedding:ServiceUrl". O backend C# não tem DeepFace embarcado.
/// </summary>
public class FaceEmbeddingServiceHttp : IFaceEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceUrl;
    private readonly ILogger<FaceEmbeddingServiceHttp> _logger;

    public FaceEmbeddingServiceHttp(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FaceEmbeddingServiceHttp> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serviceUrl = configuration["FaceEmbedding:ServiceUrl"]
            ?? "http://localhost:5001/generate-embedding";
    }

    public async Task<float[]> GerarEmbeddingAsync(string imageBase64)
    {
        if (string.IsNullOrWhiteSpace(imageBase64))
            return Array.Empty<float>();

        try
        {
            // A imagem pode vir como data URL completa ("data:image/jpeg;base64,...");
            // o serviço Python remove o prefixo antes de decodificar.
            var response = await _httpClient.PostAsJsonAsync(_serviceUrl, new { image = imageBase64 });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Serviço de embedding ({Url}) retornou status {Status}.",
                    _serviceUrl, (int)response.StatusCode);
                return Array.Empty<float>();
            }

            var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return payload?.Embedding ?? Array.Empty<float>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falha ao chamar o serviço de embedding em {Url}. O paciente ficará sem embedding.",
                _serviceUrl);
            return Array.Empty<float>();
        }
    }

    private sealed class EmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
