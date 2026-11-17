using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
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
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var pagina = await jogoService.ListarAsync(request.ParaFiltro());
        return Ok(RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
            .Sucesso(pagina.ParaResponse()));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await jogoService.BuscarPorIdAsync(id);
        return Ok(RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(detalhe.ParaListarPorIdResponse()));
    }

    // Os gêneros vão ao Service separados do jogo: ele grava os dois numa transação.
    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var criado = await jogoService.InserirAsync(request.ParaModel(), request.IdsGeneros);
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id },
            RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(criado.ParaListarPorIdResponse()));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(
        int id, [FromBody] AlterarJogoRequestApiModel request)
    {
        var alterado = await jogoService.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        return Ok(RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>.Sucesso(alterado.ParaListarPorIdResponse()));
    }

    // Não apaga: só desativa (RN03). O jogo continua no banco com st_ativo = 0.
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await jogoService.DesativarAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }
}
