using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    private const string Colunas = "co_seq_genero, no_genero";

    public GeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = $"SELECT {Colunas} FROM tb_genero ORDER BY no_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var generos = new List<Genero>();
        while (await leitor.ReadAsync())
        {
            generos.Add(Mapear(leitor));
        }
        return generos;
    }

    public async Task<Genero?> BuscarPorIdAsync(int id)
    {
        const string sql = $"SELECT {Colunas} FROM tb_genero WHERE co_seq_genero = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        // Se @idIgnorar vier preenchido (alteração), o próprio gênero não conta.
        const string sql = """
            SELECT 1 FROM tb_genero
            WHERE no_genero = @nome
              AND (@idIgnorar IS NULL OR co_seq_genero <> @idIgnorar)
            LIMIT 1
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<int> InserirAsync(Genero genero)
    {
        const string sql = "INSERT INTO tb_genero (no_genero) VALUES (@nome)";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", genero.Nome);

        await ExecutarGravacaoAsync(comando); // no_genero é UNIQUE
        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Genero genero)
    {
        const string sql = "UPDATE tb_genero SET no_genero = @nome WHERE co_seq_genero = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", genero.Id);
        AdicionarParametro(comando, "@nome", genero.Nome);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = "DELETE FROM tb_genero WHERE co_seq_genero = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    private static Genero Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_genero")),
        Nome = leitor.GetString(leitor.GetOrdinal("no_genero")),
    };
}
