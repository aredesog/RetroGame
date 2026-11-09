namespace RetroGames.Services.ServiceModels;

// Campo nulo = esse filtro não é aplicado.
public class JogoFiltroServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}
