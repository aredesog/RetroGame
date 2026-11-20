namespace RetroGames.WebApi.ApiModels;

public class ListarItemPedidoResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }   // preço pago no momento da compra
}
