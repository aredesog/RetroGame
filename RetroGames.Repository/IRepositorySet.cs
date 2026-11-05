using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

/// <summary>
/// Conjunto de repositórios: um único objeto que dá acesso a todos.
/// O Service (aula 3) recebe este objeto e usa repositorio.Plataforma, repositorio.Jogo etc.
/// </summary>
public interface IRepositorySet
{
    IPlataformaRepository Plataforma { get; }
    IJogoRepository Jogo { get; }
    IJogoGeneroRepository JogoGenero { get; }
    IGeneroRepository Genero { get; }
    ICondicaoJogoRepository CondicaoJogo { get; }
}
