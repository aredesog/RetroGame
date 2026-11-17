using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // Segura por padrão (RS03): toda rota exige login, a menos que tenha [AllowAnonymous].
            // Rota nova esquecida fica fechada, não aberta.
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Sem login: 401. Logado sem o perfil: 403.
            opcoes.AddPolicy(Politicas.Administrador, politica => politica
                .RequireAuthenticatedUser()
                .RequireRole(Perfis.Administrador));
        });

        return services;
    }
}
