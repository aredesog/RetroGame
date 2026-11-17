using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IAutenticacaoService
{
    // Nulo quando o e-mail não existe, a senha não confere ou o usuário está inativo.
    // Os três casos dão o mesmo resultado: quem tenta não descobre qual deles falhou.
    Task<Usuario?> AutenticarAsync(string email, string senha);

    // Cria o administrador inicial, mas só se ainda não existir nenhum.
    Task GarantirAdministradorInicialAsync(string email, string senha);
}
