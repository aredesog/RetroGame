namespace RetroGames.WebApi.ApiModels;

// O id da plataforma vem da rota (PUT /api/Plataforma/{id}), não do corpo.
public class AlterarPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
