using MySqlConnector;

namespace RetroGames.Repository;

// Base de todos os repositórios: conexão, comando, parâmetros e gravação.
// Não tem SQL de nenhuma tabela.
public abstract class RepositoryBase
{
    private const int ErroRegistroDuplicado = 1062;

    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // Quem chama usa "await using" para a conexão fechar mesmo se der erro.
    // Se o Service abriu um TransactionScope, a conexão entra nessa transação sozinha.
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        await conexao.OpenAsync();
        return conexao;
    }

    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;
        return comando;
    }

    // Todo valor vindo de fora entra como parâmetro, nunca concatenado no SQL (RS06).
    // null do C# precisa virar DBNull.Value para gravar NULL no banco.
    protected static void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    // Para "IN (...)": cria um parâmetro por valor (@prefixo0, @prefixo1, ...)
    // e devolve os nomes separados por vírgula. Só os nomes entram no texto do SQL.
    protected static string AdicionarParametrosLista(MySqlCommand comando, string prefixo, IEnumerable<int> valores)
    {
        var nomes = new List<string>();
        var indice = 0;
        foreach (var valor in valores)
        {
            var nome = $"@{prefixo}{indice++}";
            AdicionarParametro(comando, nome, valor);
            nomes.Add(nome);
        }
        return string.Join(", ", nomes);
    }

    // Único lugar que converte o erro 1062 (UNIQUE) em RegistroDuplicadoException.
    // Todo INSERT e UPDATE passa por aqui.
    protected static async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == ErroRegistroDuplicado)
        {
            throw new RegistroDuplicadoException("Já existe um registro com este valor.", ex);
        }
    }
}
