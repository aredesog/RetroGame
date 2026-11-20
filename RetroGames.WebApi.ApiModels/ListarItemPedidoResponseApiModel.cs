namespace RetroGames.WebApi.ApiModels;

public class ListarItemPedidoResponseApiModel
{
    public int IdJogo { get; init; }
    public string NomeJogo { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
}
