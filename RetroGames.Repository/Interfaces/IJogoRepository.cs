using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

/// <summary>Acesso a dados da tabela tb_jogo.</summary>
public interface IJogoRepository
{
    /// <summary>
    /// Os jogos de uma página e o total encontrado com o mesmo filtro.
    /// Cada filtro só vale quando informado (não nulo):
    /// busca = parte do nome; idPlataforma; ids = lista de jogos permitidos
    /// (vinda do Service, por exemplo, no filtro por gênero).
    /// Inativos só aparecem quando incluirInativos = true.
    /// </summary>
    Task<(List<Jogo> Itens, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    /// <summary>O jogo com o id informado, ou nulo se não existir.</summary>
    Task<Jogo?> BuscarPorIdAsync(int id);

    /// <summary>Grava um novo jogo e devolve o id gerado pelo banco.</summary>
    Task<int> InserirAsync(Jogo jogo);

    /// <summary>Atualiza os dados do jogo (exceto situação e data de cadastro).</summary>
    Task AlterarAsync(Jogo jogo);

    /// <summary>Marca o jogo como inativo (st_ativo = FALSE). Jogo nunca é apagado.</summary>
    Task DesativarAsync(int id);
}
