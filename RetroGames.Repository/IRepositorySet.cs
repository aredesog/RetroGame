using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

// Um único objeto com todos os repositórios. O Service recebe só este.
public interface IRepositorySet
{
    IPlataformaRepository Plataforma { get; }
    IJogoRepository Jogo { get; }
    IJogoGeneroRepository JogoGenero { get; }
    IGeneroRepository Genero { get; }
    ICondicaoJogoRepository CondicaoJogo { get; }
}
