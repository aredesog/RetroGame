namespace RetroGames.WebApi.ApiModels;

// C04: { "itens": [ { "idJogo": 3, "quantidade": 2 } ] }. Não tem preço: quem calcula é a API.
public class InserirPedidoRequestApiModel
{
    public List<InserirItemPedidoRequestApiModel> Itens { get; set; } = new();
}

public class InserirItemPedidoRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
