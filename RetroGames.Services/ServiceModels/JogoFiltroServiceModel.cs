namespace RetroGames.Services.ServiceModels;

/// <summary>
/// Filtro da listagem de jogos. Campo nulo = filtro não aplicado.
/// Ver jogos inativos (só administrador) fica para a aula 6.
/// </summary>
public class JogoFiltroServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}
