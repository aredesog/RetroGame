using RetroGames.Model.Model;

namespace RetroGames.Services.ServiceModels;

/// <summary>
/// Jogo com os dados das outras tabelas, montado com LINQ no JogoService:
/// nome da plataforma, descrição da condição e gêneros.
/// </summary>
public class JogoDetalheServiceModel
{
    public Jogo Jogo { get; set; } = new();
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new();
}
