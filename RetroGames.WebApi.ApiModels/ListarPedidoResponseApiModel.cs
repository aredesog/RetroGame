namespace RetroGames.WebApi.ApiModels;

// Formato "Pedido" do contrato do grupo (00-CONTEXTO-GERAL, seção 5).
public class ListarPedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }   // UTC: o JSON sai com "Z" no fim
    public int IdSituacao { get; set; }
    public string DescricaoSituacao { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public List<ListarItemPedidoResponseApiModel> Itens { get; set; } = new();
}
