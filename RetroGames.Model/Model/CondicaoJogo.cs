namespace RetroGames.Model.Model;

// Tabela tb_condicao_jogo (domínio: a chave é fixa, não é gerada pelo banco)
public class CondicaoJogo
{
    public int Id { get; set; }                               // co_condicao_jogo
    public string Descricao { get; set; } = string.Empty;     // ds_condicao_jogo
}
