using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);

    Task<Usuario?> BuscarPorIdAsync(int id);

    Task<List<Usuario>> ListarPorIdsAsync(IReadOnlyCollection<int> ids);

    Task<bool> ExisteAdministradorAsync();

    // Devolve o id gerado pelo banco.
    Task<int> InserirAsync(Usuario usuario);
}
