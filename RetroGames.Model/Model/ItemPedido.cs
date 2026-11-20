namespace RetroGames.Model.Model;

// Tabela tb_item_pedido
public class ItemPedido
{
    public int Id { get; set; }                 // co_seq_item_pedido
    public int Quantidade { get; set; }         // qt_item
    public decimal ValorUnitario { get; set; }  // vl_unitario
    public int IdPedido { get; set; }          // co_pedido
    public int IdJogo { get; set; }            // co_jogo
}
