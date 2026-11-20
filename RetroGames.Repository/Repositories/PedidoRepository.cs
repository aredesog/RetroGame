using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    private const string Consulta = "SELECT co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido FROM tb_pedido";
    private const string Ordem = " ORDER BY dt_pedido DESC, co_seq_pedido DESC";

    public PedidoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, "INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido) VALUES (@data, @total, @usuario, @situacao)");
        AdicionarParametro(comando, "@data", pedido.Data);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@usuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@situacao", pedido.IdSituacaoPedido);
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    public async Task<Pedido?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, Consulta + " WHERE co_seq_pedido = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, Consulta + " WHERE co_usuario = @usuario" + Ordem);
        AdicionarParametro(comando, "@usuario", idUsuario);
        var itens = new List<Pedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(Mapear(leitor));
        }

        return itens;
    }

    public async Task<List<Pedido>> ListarAsync(int? idSituacao)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, Consulta + " WHERE (@situacao IS NULL OR co_situacao_pedido = @situacao)" + Ordem);
        AdicionarParametro(comando, "@situacao", idSituacao);
        var itens = new List<Pedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(Mapear(leitor));
        }

        return itens;
    }

    public async Task AlterarSituacaoAsync(int id, int idSituacao)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, "UPDATE tb_pedido SET co_situacao_pedido = @situacao WHERE co_seq_pedido = @id");
        AdicionarParametro(comando, "@id", id);
        AdicionarParametro(comando, "@situacao", idSituacao);
        await ExecutarGravacaoAsync(comando);
    }

    private static Pedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32(0),
        Data = DateTime.SpecifyKind(leitor.GetDateTime(1), DateTimeKind.Utc),
        Total = leitor.GetDecimal(2),
        IdUsuario = leitor.GetInt32(3),
        IdSituacaoPedido = leitor.GetInt32(4),
    };
}
