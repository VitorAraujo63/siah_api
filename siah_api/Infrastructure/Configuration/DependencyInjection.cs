using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SiahApi.Application.Services;
using SiahApi.Application.UseCases;
using SiahApi.Domain.Ports.Input;
using SiahApi.Domain.Ports.Output;
using SiahApi.Infrastructure.Adapters.Output;
using SiahApi.Infrastructure.Services;

namespace SiahApi.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Supabase:ConnectionStrings:DefaultConnection"]
            ?? configuration["Supabase:ConnectionString"]
            ?? throw new InvalidOperationException("Supabase:ConnectionString não configurada. Consulte local_int_db.txt.");

        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        services.AddScoped<IPacienteRepository, SupabasePacienteRepository>();
        services.AddScoped<ITriagemRepository, SupabaseTriagemRepository>();
        services.AddScoped<IBiometriaRepository, SupabaseBiometriaRepository>();
        services.AddScoped<IAuthRepository, SupabaseAuthRepository>();
        services.AddScoped<IAgendamentoRepository, SupabaseAgendamentoRepository>();
        services.AddScoped<IMedicoRepository, SupabaseMedicoRepository>();
        services.AddScoped<IEspecialidadeRepository, SupabaseEspecialidadeRepository>();
        services.AddScoped<IFilaRepository, SupabaseFilaRepository>();
        services.AddScoped<IHistoricoRepository, SupabaseHistoricoRepository>();
        services.AddScoped<IDocumentoRepository, SupabaseDocumentoRepository>();

        services.AddScoped<IFaceEmbeddingService, FaceEmbeddingServiceStub>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPacienteUseCase, PacienteUseCase>();
        services.AddScoped<ITriagemUseCase, TriagemUseCase>();
        services.AddScoped<IBiometriaUseCase, BiometriaUseCase>();
        services.AddScoped<IAuthUseCase, AuthUseCase>();
        services.AddScoped<IPerfilUseCase, PerfilUseCase>();
        services.AddScoped<IAgendamentoUseCase, AgendamentoUseCase>();
        services.AddScoped<IMedicoUseCase, MedicoUseCase>();
        services.AddScoped<IEspecialidadeUseCase, EspecialidadeUseCase>();
        services.AddScoped<IFilaUseCase, FilaUseCase>();
        services.AddScoped<IHistoricoUseCase, HistoricoUseCase>();
        services.AddScoped<IDocumentoUseCase, DocumentoUseCase>();

        return services;
    }
}
