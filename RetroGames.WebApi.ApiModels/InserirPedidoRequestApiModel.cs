namespace RetroGames.WebApi.ApiModels;

public class InserirPedidoRequestApiModel
{
    public List<InserirItemPedidoRequestApiModel> Itens { get; set; } = [];
}
