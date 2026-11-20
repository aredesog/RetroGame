using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

// C04 a C07. Nenhuma rota daqui é pública: sem atributo, vale a política padrão (usuário logado).
[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        this.pedidoService = pedidoService;
    }

    // C04: finaliza a compra do carrinho. Sem login: 401 com código 4 (política padrão).
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPedidoResponseApiModel>>> Inserir(
        [FromBody] InserirPedidoRequestApiModel request)
    {
        var itens = request.Itens.Select(i => (i.IdJogo, i.Quantidade)).ToList();
        var pedido = await pedidoService.InserirAsync(itens);
        return CreatedAtAction(nameof(ListarPorId), new { id = pedido.Id },
            RetroGamesResponseApiModel<ListarPedidoResponseApiModel>.Sucesso(pedido.ParaResponse()));
    }

    // C05: pedidos do cliente logado.
    [HttpGet("meus")]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>>> ListarMeus()
    {
        var pedidos = await pedidoService.ListarMeusAsync();
        return Ok(RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>
            .Sucesso(pedidos.Select(p => p.ParaResponse()).ToList()));
    }

    // C07: GET /api/Pedido?idSituacao=2. Sem idSituacao, traz todos.
    [HttpGet]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>>> Listar(
        [FromQuery] int? idSituacao)
    {
        var pedidos = await pedidoService.ListarAsync(idSituacao);
        return Ok(RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>
            .Sucesso(pedidos.Select(p => p.ParaResponse()).ToList()));
    }

    // C06: sem atributo, vale a política padrão (qualquer usuário logado).
    // A regra "só o dono ou o administrador" fica no PedidoService (403 com código 113).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPedidoResponseApiModel>>> ListarPorId(int id)
    {
        var pedido = await pedidoService.BuscarPorIdAsync(id);
        return Ok(RetroGamesResponseApiModel<ListarPedidoResponseApiModel>.Sucesso(pedido.ParaResponse()));
    }

    // C07: PUT /api/Pedido/5/situacao com { "idSituacao": 2 }.
    [HttpPut("{id:int}/situacao")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPedidoResponseApiModel>>> AlterarSituacao(
        int id, [FromBody] AlterarSituacaoPedidoRequestApiModel request)
    {
        var pedido = await pedidoService.AlterarSituacaoAsync(id, request.IdSituacao);
        return Ok(RetroGamesResponseApiModel<ListarPedidoResponseApiModel>.Sucesso(pedido.ParaResponse()));
    }
}
