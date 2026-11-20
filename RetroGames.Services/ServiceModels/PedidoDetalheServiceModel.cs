namespace RetroGames.Services.ServiceModels;

public class PedidoDetalheServiceModel
{
    public int Id { get; init; }
    public DateTime Data { get; init; }
    public int IdSituacao { get; init; }
    public string DescricaoSituacao { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public int IdUsuario { get; init; }
    public string? NomeUsuario { get; init; }
    public List<ItemPedidoDetalheServiceModel> Itens { get; init; } = [];
}
