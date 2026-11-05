using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

/// <summary>Todo o SQL da tabela tb_jogo.</summary>
public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = """
        co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
        qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
        """;

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(List<Jogo> Itens, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        // Lista de ids informada, mas vazia: nenhum jogo pode aparecer.
        if (ids is not null && ids.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        await using var conexao = await AbrirConexaoAsync();

        // 1) Total com o filtro
        await using var comandoTotal = CriarComando(conexao, string.Empty);
        comandoTotal.CommandText = $"SELECT COUNT(*) FROM tb_jogo {MontarWhere(comandoTotal, busca, idPlataforma, ids, incluirInativos)}";
        var total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());

        // 2) Itens da página com o mesmo filtro
        await using var comandoPagina = CriarComando(conexao, string.Empty);
        comandoPagina.CommandText = $"""
            SELECT {Colunas}
            FROM tb_jogo
            {MontarWhere(comandoPagina, busca, idPlataforma, ids, incluirInativos)}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @deslocamento
            """;
        AdicionarParametro(comandoPagina, "@limite", tamanhoPagina);
        AdicionarParametro(comandoPagina, "@deslocamento", (pagina - 1) * tamanhoPagina);

        var itens = new List<Jogo>();
        await using var leitor = await comandoPagina.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(Mapear(leitor));
        }

        return (itens, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = $"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
                 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @anoLancamento, @preco, @estoque,
                 @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    /// <summary>
    /// Monta o WHERE da listagem e adiciona os parâmetros ao comando.
    /// Usado pela contagem e pela página, para os dois terem o mesmo filtro.
    /// </summary>
    private static string MontarWhere(
        MySqlCommand comando,
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos)
    {
        var condicoes = new List<string>
        {
            "(@incluirInativos = TRUE OR st_ativo = TRUE)",
            "(@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))",
            "(@idPlataforma IS NULL OR co_plataforma = @idPlataforma)",
        };
        AdicionarParametro(comando, "@incluirInativos", incluirInativos);
        AdicionarParametro(comando, "@busca", string.IsNullOrWhiteSpace(busca) ? null : busca.Trim());
        AdicionarParametro(comando, "@idPlataforma", idPlataforma);

        if (ids is not null)
        {
            condicoes.Add($"co_seq_jogo IN ({AdicionarParametrosLista(comando, "idJogo", ids)})");
        }

        return "WHERE " + string.Join(" AND ", condicoes);
    }

    /// <summary>Converte a linha atual do leitor num objeto Jogo, coluna por coluna.</summary>
    private static Jogo Mapear(MySqlDataReader leitor)
    {
        var colunaDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_jogo")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_jogo")),
            Descricao = leitor.IsDBNull(colunaDescricao) ? null : leitor.GetString(colunaDescricao),
            AnoLancamento = leitor.GetInt32(leitor.GetOrdinal("nu_ano_lancamento")),
            Preco = leitor.GetDecimal(leitor.GetOrdinal("vl_preco")),
            Estoque = leitor.GetInt32(leitor.GetOrdinal("qt_estoque")),
            Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
            DataCadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
            IdPlataforma = leitor.GetInt32(leitor.GetOrdinal("co_plataforma")),
            IdCondicaoJogo = leitor.GetInt32(leitor.GetOrdinal("co_condicao_jogo")),
        };
    }
}
