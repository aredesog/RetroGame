namespace RetroGames.Model.Model;

// Tabela tb_perfil (domínio: a chave é fixa, não é gerada pelo banco)
public class Perfil
{
    public int Id { get; set; }                               // co_perfil
    public string Descricao { get; set; } = string.Empty;     // ds_perfil
}
