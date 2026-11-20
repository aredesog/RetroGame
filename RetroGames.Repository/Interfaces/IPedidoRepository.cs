using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido);
    Task<Pedido?> BuscarPorIdAsync(int id);
    Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario);
    Task<List<Pedido>> ListarAsync(int? idSituacao);
    Task AlterarSituacaoAsync(int id, int idSituacao);
}
