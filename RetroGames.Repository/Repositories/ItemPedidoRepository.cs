using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{

    public ItemPedidoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task InserirAsync(ItemPedido item)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, "INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) VALUES (@quantidade, @valor, @pedido, @jogo)");
        AdicionarParametro(comando, "@quantidade", item.Quantidade);
        AdicionarParametro(comando, "@valor", item.ValorUnitario);
        AdicionarParametro(comando, "@pedido", item.IdPedido);
        AdicionarParametro(comando, "@jogo", item.IdJogo);
        await ExecutarGravacaoAsync(comando);
    }

    public async Task<List<ItemPedido>> ListarPorPedidosAsync(IReadOnlyCollection<int> idsPedidos)
    {
        if (idsPedidos.Count == 0) return [];

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        var parametros = AdicionarParametrosLista(comando, "pedido", idsPedidos);
        comando.CommandText = $"""
            SELECT co_seq_item_pedido, qt_item, vl_unitario, co_pedido, co_jogo
            FROM tb_item_pedido WHERE co_pedido IN ({parametros}) ORDER BY co_seq_item_pedido
            """;
        var itens = new List<ItemPedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(Mapear(leitor));
        }

        return itens;
    }

    private static ItemPedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32(0),
        Quantidade = leitor.GetInt32(1),
        ValorUnitario = leitor.GetDecimal(2),
        IdPedido = leitor.GetInt32(3),
        IdJogo = leitor.GetInt32(4),
    };
}
