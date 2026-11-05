using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

/// <summary>Todo o SQL da tabela tb_jogo_genero.</summary>
public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = "SELECT co_jogo, co_genero FROM tb_jogo_genero";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        return await LerVinculosAsync(comando);
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        comando.CommandText =
            $"SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN ({AdicionarParametrosLista(comando, "idJogo", idsJogos)})";

        return await LerVinculosAsync(comando);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        AdicionarParametro(comando, "@idJogo", idJogo);

        // Um único INSERT com uma linha por gênero: (@idJogo, @idGenero0), (@idJogo, @idGenero1)...
        var linhas = new List<string>();
        var indice = 0;
        foreach (var idGenero in idsGeneros)
        {
            var nome = $"@idGenero{indice++}";
            AdicionarParametro(comando, nome, idGenero);
            linhas.Add($"(@idJogo, {nome})");
        }
        comando.CommandText = $"INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES {string.Join(", ", linhas)}";

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    private static async Task<List<JogoGenero>> LerVinculosAsync(MySqlCommand comando)
    {
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(leitor.GetOrdinal("co_jogo")),
                IdGenero = leitor.GetInt32(leitor.GetOrdinal("co_genero")),
            });
        }
        return vinculos;
    }
}
