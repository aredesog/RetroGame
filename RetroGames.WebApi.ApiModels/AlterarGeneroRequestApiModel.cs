namespace RetroGames.WebApi.ApiModels;

// O id do gênero vem da rota (PUT /api/Genero/{id}), não do corpo.
public class AlterarGeneroRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
}
