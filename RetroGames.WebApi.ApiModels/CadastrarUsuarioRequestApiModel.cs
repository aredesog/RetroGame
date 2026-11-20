namespace RetroGames.WebApi.ApiModels;

// C01: o perfil não vem do cliente. O Service sempre cadastra como Cliente.
public class CadastrarUsuarioRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
