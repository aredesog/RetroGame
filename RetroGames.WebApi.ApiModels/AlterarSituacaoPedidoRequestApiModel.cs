namespace RetroGames.WebApi.ApiModels;

// O id do pedido vem da rota (PUT /api/Pedido/{id}/situacao), não do corpo.
public class AlterarSituacaoPedidoRequestApiModel
{
    public int IdSituacao { get; set; }
}
