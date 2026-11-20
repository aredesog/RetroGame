using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
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
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await generoService.ListarAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
            .Sucesso(generos.Select(g => g.ParaResponse()).ToList()));
    }

    // Público pela tabela de acesso do AGENTS.md. Id inexistente: 404 com código 105.
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel>>> ListarPorId(int id)
    {
        var genero = await generoService.BuscarPorIdAsync(id);
        return Ok(RetroGamesResponseApiModel<ListarGeneroResponseApiModel>.Sucesso(genero.ParaResponse()));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel>>> Inserir(
        [FromBody] InserirGeneroRequestApiModel request)
    {
        var criado = await generoService.InserirAsync(request.ParaModel());
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Id },
            RetroGamesResponseApiModel<ListarGeneroResponseApiModel>.Sucesso(criado.ParaResponse()));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel>>> Alterar(
        int id, [FromBody] AlterarGeneroRequestApiModel request)
    {
        var alterado = await generoService.AlterarAsync(request.ParaModel(id));
        return Ok(RetroGamesResponseApiModel<ListarGeneroResponseApiModel>.Sucesso(alterado.ParaResponse()));
    }

    // 200 com o envelope, nunca 204 (AGENTS.md).
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await generoService.ExcluirAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }
}
