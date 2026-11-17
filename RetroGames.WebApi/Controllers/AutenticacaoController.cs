using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService autenticacaoService;

    public AutenticacaoController(IAutenticacaoService autenticacaoService)
    {
        this.autenticacaoService = autenticacaoService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        [FromBody] LoginRequestApiModel request)
    {
        var usuario = await autenticacaoService.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
            });
        }

        // Cria o cookie retrogames.sessao com as claims do usuário.
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.CriarClaimsPrincipal());

        return Ok(RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>.Sucesso(ParaResponse(usuario)));
    }

    // Exige login pela política padrão (FallbackPolicy): não precisa de [Authorize].
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }

    // Lê os dados direto das claims do cookie, sem ir ao banco.
    [HttpGet("eu")]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        var resposta = new UsuarioLogadoResponseApiModel
        {
            Id = User.ObterIdUsuario() ?? 0,
            Nome = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Perfil = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
        };
        return Ok(RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>.Sucesso(resposta));
    }

    private static UsuarioLogadoResponseApiModel ParaResponse(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = ClaimsPrincipalExtensions.NomeDoPerfil(usuario.IdPerfil),
    };
}
