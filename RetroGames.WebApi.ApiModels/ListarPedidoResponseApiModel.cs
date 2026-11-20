namespace RetroGames.WebApi.ApiModels;

public class ListarPedidoResponseApiModel
{
    public int Id { get; init; }
    public DateTime Data { get; init; }
    public int IdSituacao { get; init; }
    public string DescricaoSituacao { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public int IdUsuario { get; init; }
    public string? NomeUsuario { get; init; }
    public List<ListarItemPedidoResponseApiModel> Itens { get; init; } = [];
}
