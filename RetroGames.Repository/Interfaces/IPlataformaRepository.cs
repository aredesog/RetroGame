using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync();

    Task<Plataforma?> BuscarPorIdAsync(int id);

    // idIgnorar: na alteração, passa o id da própria plataforma para ela não contar como repetida.
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);

    Task<bool> PossuiJogosAsync(int id);

    // Devolve o id gerado pelo banco.
    Task<int> InserirAsync(Plataforma plataforma);

    Task AlterarAsync(Plataforma plataforma);

    Task ExcluirAsync(int id);
}
