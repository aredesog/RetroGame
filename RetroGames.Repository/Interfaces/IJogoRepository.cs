using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository
{
    // Devolve os jogos da página e o total encontrado com o mesmo filtro.
    // Filtro nulo = não aplicado. "ids" limita a busca a esses jogos
    // (o Service usa para o filtro por gênero).
    Task<(List<Jogo> Itens, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        IReadOnlyCollection<int>? ids,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    Task<Jogo?> BuscarPorIdAsync(int id);

    // Devolve o id gerado pelo banco.
    Task<int> InserirAsync(Jogo jogo);

    // Não altera st_ativo nem dt_cadastro.
    Task AlterarAsync(Jogo jogo);

    // Exclusão lógica (RN03): jogo nunca é apagado.
    Task DesativarAsync(int id);
}
