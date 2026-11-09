using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarTodosAsync();

    // Uma única consulta para todos os jogos da lista.
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos);

    Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros);

    // Usado na alteração: apaga os vínculos antigos antes de gravar os novos.
    Task RemoverPorJogoAsync(int idJogo);
}
