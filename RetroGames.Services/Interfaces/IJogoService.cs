using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IJogoService
{
    Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    Task DesativarAsync(int id);
}
