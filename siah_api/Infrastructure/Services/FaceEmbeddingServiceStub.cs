using SiahApi.Application.Services;

namespace SiahApi.Infrastructure.Services;

public class FaceEmbeddingServiceStub : IFaceEmbeddingService
{
    public Task<float[]> GerarEmbeddingAsync(string imageBase64)
    {
        return Task.FromResult(Array.Empty<float>());
    }
}
