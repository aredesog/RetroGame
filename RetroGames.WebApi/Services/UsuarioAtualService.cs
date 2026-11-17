using RetroGames.Model.Constantes;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public bool EstaAutenticado => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public int? IdUsuario => httpContextAccessor.HttpContext?.User.ObterIdUsuario();

    public bool EhAdministrador => httpContextAccessor.HttpContext?.User.IsInRole(Perfis.Administrador) ?? false;
}
