using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService jogoService;

    public JogoController(IJogoService jogoService)
    {
        this.jogoService = jogoService;
    }

    // [FromQuery]: os filtros vêm da query string (?busca=...&pagina=...), não do corpo.
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var pagina = await jogoService.ListarAsync(request.ParaFiltro());
        return Ok(RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
            .Sucesso(pagina.ParaResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await jogoService.BuscarPorIdAsync(id);
        return Ok(RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(detalhe.ParaListarPorIdResponse()));
    }
}
