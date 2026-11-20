using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();

    Task<Genero?> BuscarPorIdAsync(int id);

    // idIgnorar: na alteração, passa o id do próprio gênero para ele não contar como repetido.
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);

    // Devolve o id gerado pelo banco.
    Task<int> InserirAsync(Genero genero);

    Task AlterarAsync(Genero genero);

    Task ExcluirAsync(int id);
}
