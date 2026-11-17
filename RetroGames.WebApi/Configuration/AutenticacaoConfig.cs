using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacaoPorCookie(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;                  // o JavaScript da página não consegue ler o cookie
                opcoes.Cookie.SameSite = SameSiteMode.Strict;   // não vai em requisições vindas de outro site (defesa contra CSRF)
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;                // cada uso renova o prazo

                // O padrão do cookie é redirecionar para uma página de login (HTML).
                // Numa API, o certo é responder 401/403 no envelope.
                opcoes.Events.OnRedirectToLogin = contexto =>
                    EscreverErroAsync(contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    EscreverErroAsync(contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task EscreverErroAsync(HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage,
        });
    }
}
