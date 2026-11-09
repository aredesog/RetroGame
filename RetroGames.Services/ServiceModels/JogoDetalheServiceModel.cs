using RetroGames.Model.Model;

namespace RetroGames.Services.ServiceModels;

// Jogo já combinado com os dados das outras tabelas (montado no JogoService).
public class JogoDetalheServiceModel
{
    public Jogo Jogo { get; set; } = new();
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new();
}
