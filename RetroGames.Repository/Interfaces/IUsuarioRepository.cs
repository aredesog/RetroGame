using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);

    Task<Usuario?> BuscarPorIdAsync(int id);

    Task<bool> ExisteAdministradorAsync();

    // Devolve o id gerado pelo banco.
    Task<int> InserirAsync(Usuario usuario);
}
