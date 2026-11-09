using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

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
        // Lista de ids vazia (ex.: gênero sem jogos): nem consulta o banco, não há resultado.
        if (ids is not null && ids.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        await using var conexao = await AbrirConexaoAsync();

        // O total e a página usam o mesmo WHERE, então sempre batem.
        await using var comandoTotal = CriarComando(conexao, string.Empty);
        comandoTotal.CommandText = $"SELECT COUNT(*) FROM tb_jogo {MontarWhere(comandoTotal, busca, idPlataforma, ids, incluirInativos)}";
        var total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());

        await using var comandoPagina = CriarComando(conexao, string.Empty);
        comandoPagina.CommandText = $"""
            SELECT {Colunas}
            FROM tb_jogo
            {MontarWhere(comandoPagina, busca, idPlataforma, ids, incluirInativos)}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @deslocamento
            """;
        AdicionarParametro(comandoPagina, "@limite", tamanhoPagina);
        AdicionarParametro(comandoPagina, "@deslocamento", (pagina - 1) * tamanhoPagina); // página 1 começa no 0

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

    // Cada condição tem a forma "(@filtro IS NULL OR coluna = @filtro)":
    // se o filtro não foi informado, a condição vira sempre verdadeira e é ignorada.
    // Adiciona os parâmetros no comando recebido e devolve o texto do WHERE.
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

    private static Jogo Mapear(MySqlDataReader leitor)
    {
        // ds_jogo aceita NULL: precisa conferir antes de ler como texto.
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
