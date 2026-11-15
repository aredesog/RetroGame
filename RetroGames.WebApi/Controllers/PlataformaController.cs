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
}
