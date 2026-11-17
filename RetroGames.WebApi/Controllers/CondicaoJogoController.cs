using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService condicaoJogoService;

    public CondicaoJogoController(ICondicaoJogoService condicaoJogoService)
    {
        this.condicaoJogoService = condicaoJogoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>> Listar()
    {
        var condicoes = await condicaoJogoService.ListarAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
            .Sucesso(condicoes.Select(c => c.ParaResponse()).ToList()));
    }
}
