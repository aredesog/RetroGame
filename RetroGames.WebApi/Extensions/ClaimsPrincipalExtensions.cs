using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

// Claims são os dados do usuário guardados (criptografados) dentro do cookie de sessão.
public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal CriarClaimsPrincipal(this Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, NomeDoPerfil(usuario.IdPerfil)), // usada pela política SomenteAdministrador
        };

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identidade);
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }

    public static string NomeDoPerfil(int idPerfil) =>
        idPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
}
