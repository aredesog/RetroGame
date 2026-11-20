using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        this.pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPedidoResponseApiModel>>> Inserir(
        [FromBody] InserirPedidoRequestApiModel request)
    {
        var itens = request.Itens.Select(item => (item.IdJogo, item.Quantidade)).ToArray();
        var pedido = await pedidoService.InserirAsync(itens);
        return StatusCode(StatusCodes.Status201Created,
            RetroGamesResponseApiModel<ListarPedidoResponseApiModel>.Sucesso(pedido.ParaResponse()));
    }

    [HttpGet("meus")]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>>> ListarMeus()
    {
        var pedidos = await pedidoService.ListarMeusAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>
            .Sucesso(pedidos.Select(pedido => pedido.ParaResponse()).ToList()));
    }
}
