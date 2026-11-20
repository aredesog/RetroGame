using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    private const string Consulta = "SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido";

    public SituacaoPedidoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, Consulta + " ORDER BY co_situacao_pedido");
        var itens = new List<SituacaoPedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(Mapear(leitor));
        }

        return itens;
    }

    public async Task<SituacaoPedido?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, Consulta + " WHERE co_situacao_pedido = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    private static SituacaoPedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32(0),
        Descricao = leitor.GetString(1),
    };
}
