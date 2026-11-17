using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositories;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas =
        "co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil";

    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        const string sql = $"SELECT {Colunas} FROM tb_usuario WHERE ds_email = @email";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@email", email);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = $"SELECT {Colunas} FROM tb_usuario WHERE co_seq_usuario = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = "SELECT 1 FROM tb_usuario WHERE co_perfil = @idPerfil LIMIT 1";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idPerfil", PerfilIds.Administrador);

        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES
                (@nome, @email, @senhaHash, @ativo, @dataCadastro, @idPerfil)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@dataCadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando); // ds_email é UNIQUE
        return (int)comando.LastInsertedId;
    }

    private static Usuario Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_usuario")),
        Nome = leitor.GetString(leitor.GetOrdinal("no_usuario")),
        Email = leitor.GetString(leitor.GetOrdinal("ds_email")),
        SenhaHash = leitor.GetString(leitor.GetOrdinal("ds_senha_hash")),
        Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
        DataCadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
        IdPerfil = leitor.GetInt32(leitor.GetOrdinal("co_perfil")),
    };
}
