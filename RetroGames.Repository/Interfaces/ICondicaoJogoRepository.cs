using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

/// <summary>Acesso a dados da tabela tb_condicao_jogo (domínio, somente leitura).</summary>
public interface ICondicaoJogoRepository
{
    /// <summary>Todas as condições de jogo, na ordem da chave fixa.</summary>
    Task<List<CondicaoJogo>> ListarAsync();
}
