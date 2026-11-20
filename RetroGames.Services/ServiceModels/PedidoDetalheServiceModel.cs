namespace RetroGames.Services.ServiceModels;

// Pedido já combinado com os dados das outras tabelas (montado no PedidoService.MontarDetalhesAsync).
public class PedidoDetalheServiceModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }   // UTC
    public int IdSituacao { get; set; }
    public string DescricaoSituacao { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public List<ItemPedidoDetalheServiceModel> Itens { get; set; } = new();
}

public class ItemPedidoDetalheServiceModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }   // preço pago, gravado no item (não o preço atual do jogo)
}
