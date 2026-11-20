using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> InserirAsync(IReadOnlyCollection<(int IdJogo, int Quantidade)> itens);
    Task<List<PedidoDetalheServiceModel>> ListarMeusAsync();
}
