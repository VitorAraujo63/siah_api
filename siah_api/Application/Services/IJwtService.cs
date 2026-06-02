namespace SiahApi.Application.Services;

public interface IJwtService
{
    string GerarAccessToken(Guid userId, string cpf, string nome);
    string GerarRefreshToken();
    Guid? ObterUserIdDoToken(string token);
}
