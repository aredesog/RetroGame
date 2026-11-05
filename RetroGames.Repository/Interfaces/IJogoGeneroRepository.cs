using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

/// <summary>Acesso a dados da tabela tb_jogo_genero (vínculos jogo × gênero).</summary>
public interface IJogoGeneroRepository
{
    /// <summary>Todos os vínculos entre jogos e gêneros.</summary>
    Task<List<JogoGenero>> ListarTodosAsync();

    /// <summary>Os vínculos de uma lista de jogos, numa única consulta.</summary>
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos);

    /// <summary>Grava os vínculos de um jogo com os seus gêneros.</summary>
    Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>Apaga os vínculos de um jogo, para regravá-los na alteração.</summary>
    Task RemoverPorJogoAsync(int idJogo);
}
