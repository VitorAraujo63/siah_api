namespace SiahApi.Application.Services;

public interface IFaceEmbeddingService
{
    Task<float[]> GerarEmbeddingAsync(string imageBase64);
}
