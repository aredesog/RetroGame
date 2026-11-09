using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

/// <summary>Regras de negócio de jogos (RN03 a RN05) e montagem do catálogo.</summary>
public interface IJogoService
{
    /// <summary>Página do catálogo com o detalhe de cada jogo e o total encontrado.</summary>
    Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro);

    /// <summary>Lança JOGO_NAO_ENCONTRADO se não existir.</summary>
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    /// <summary>Grava o jogo e os gêneros numa transação (RN05).</summary>
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>Grava o jogo e troca os gêneros numa transação (RN05).</summary>
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);

    /// <summary>Exclusão lógica: marca o jogo como inativo (RN03).</summary>
    Task DesativarAsync(int id);
}
