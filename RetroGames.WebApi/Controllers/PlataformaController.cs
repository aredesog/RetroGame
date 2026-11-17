using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]   // [controller] = nome da classe sem "Controller": /api/Plataforma
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        this.plataformaService = plataformaService;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await plataformaService.ListarAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
            .Sucesso(plataformas.Select(p => p.ParaResponse()).ToList()));
    }

    // Id inexistente: o Service lança BusinessRuleException e o GlobalExceptionHandler responde 404.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
    {
        var plataforma = await plataformaService.BuscarPorIdAsync(id);
        return Ok(RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>.Sucesso(plataforma.ParaResponse()));
    }

    // 201 Created, com o cabeçalho Location apontando para GET /api/Plataforma/{id}.
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(
        [FromBody] InserirPlataformaRequestApiModel request)
    {
        var criada = await plataformaService.InserirAsync(request.ParaModel());
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id },
            RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>.Sucesso(criada.ParaResponse()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(
        int id, [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var alterada = await plataformaService.AlterarAsync(request.ParaModel(id));
        return Ok(RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>.Sucesso(alterada.ParaResponse()));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await plataformaService.ExcluirAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }
}
