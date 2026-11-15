using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Repositories;
using RetroGames.Services;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Configuration;

// Diz ao contêiner de injeção de dependência qual classe entregar para cada interface.
// Singleton: um único objeto para a aplicação toda.
// Scoped: um objeto novo por requisição HTTP, descartado no fim dela.
public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IJogoService, JogoService>();

        return services;
    }
}
