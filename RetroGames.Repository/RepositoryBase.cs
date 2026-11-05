using MySqlConnector;

namespace RetroGames.Repository;

/// <summary>
/// Base de todos os repositórios: conexão, comando, parâmetros e gravação.
/// Não contém SQL de nenhuma tabela.
/// </summary>
public abstract class RepositoryBase
{
    /// <summary>Código do MySQL para valor repetido em coluna UNIQUE.</summary>
    private const int ErroRegistroDuplicado = 1062;

    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Abre uma conexão com o banco. Quem chama usa "await using",
    /// para a conexão ser fechada mesmo em caso de erro.
    /// Dentro de um TransactionScope aberto pelo Service, a conexão
    /// participa da transação automaticamente.
    /// </summary>
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        await conexao.OpenAsync();
        return conexao;
    }

    /// <summary>Cria um comando com o texto SQL informado.</summary>
    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;
        return comando;
    }

    /// <summary>
    /// Adiciona um parâmetro (@nome) ao comando. Valor nulo vira NULL no banco.
    /// Todo valor vindo de fora entra por aqui, nunca concatenado no SQL (RS06).
    /// </summary>
    protected static void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    /// <summary>
    /// Executa uma gravação (INSERT ou UPDATE) e devolve o número de linhas afetadas.
    /// É o único lugar que converte o erro 1062 em RegistroDuplicadoException.
    /// </summary>
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
