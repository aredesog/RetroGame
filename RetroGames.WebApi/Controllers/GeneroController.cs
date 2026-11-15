using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService generoService;

    public GeneroController(IGeneroService generoService)
    {
        this.generoService = generoService;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await generoService.ListarAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
            .Sucesso(generos.Select(g => g.ParaResponse()).ToList()));
    }
}
