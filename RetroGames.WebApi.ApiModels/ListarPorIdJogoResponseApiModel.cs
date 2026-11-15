namespace RetroGames.WebApi.ApiModels;

// Detalhe do jogo: os dados da listagem, mais a descrição, os ids das referências
// (o formulário de edição precisa deles) e os gêneros com id e nome.
public class ListarPorIdJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public int IdPlataforma { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public int IdCondicaoJogo { get; set; }
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<ListarGeneroResponseApiModel> Generos { get; set; } = new();
}
