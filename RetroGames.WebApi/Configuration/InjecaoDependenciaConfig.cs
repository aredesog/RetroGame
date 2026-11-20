using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Repositories;
using RetroGames.Services;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.Services;

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
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IItemPedidoRepository, ItemPedidoRepository>();
        services.AddScoped<ISituacaoPedidoRepository, SituacaoPedidoRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        // PasswordHasher não guarda estado: um único objeto serve para toda a aplicação.
        services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        // Usuário da requisição atual, lido do cookie. O Service só conhece a interface.
        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

        return services;
    }
}
