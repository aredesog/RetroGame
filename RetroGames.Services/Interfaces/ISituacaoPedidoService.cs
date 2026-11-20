using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface ISituacaoPedidoService
{
    Task<List<SituacaoPedido>> ListarAsync();
}
