using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SituacaoPedidoController : ControllerBase
{
    private readonly ISituacaoPedidoService situacaoPedidoService;

    public SituacaoPedidoController(ISituacaoPedidoService situacaoPedidoService)
    {
        this.situacaoPedidoService = situacaoPedidoService;
    }

    // Diferente das condições de jogo, esta lista não é pública: pela tabela de acesso
    // do AGENTS.md, GET /api/SituacaoPedido é só do administrador (cliente recebe 403/5).
    [HttpGet]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarSituacaoPedidoResponseApiModel>>>> Listar()
    {
        var situacoes = await situacaoPedidoService.ListarAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarSituacaoPedidoResponseApiModel>>
            .Sucesso(situacoes.Select(s => s.ParaResponse()).ToList()));
    }
}
