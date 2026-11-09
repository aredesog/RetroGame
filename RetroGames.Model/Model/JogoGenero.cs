namespace RetroGames.Model.Model;

// Tabela tb_jogo_genero: associação N:N entre jogo e gênero (chave composta)
public class JogoGenero
{
    public int IdJogo { get; set; }                           // co_jogo
    public int IdGenero { get; set; }                         // co_genero
}
