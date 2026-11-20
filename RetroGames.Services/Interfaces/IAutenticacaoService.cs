using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IAutenticacaoService
{
    // Nulo quando o e-mail não existe, a senha não confere ou o usuário está inativo.
    // Os três casos dão o mesmo resultado: quem tenta não descobre qual deles falhou.
    Task<Usuario?> AutenticarAsync(string email, string senha);

    // Cria o administrador inicial, mas só se ainda não existir nenhum.
    Task GarantirAdministradorInicialAsync(string email, string senha);

    // C01: cria um cliente ativo, com senha em hash. E-mail já usado → EMAIL_JA_CADASTRADO (106).
    Task<Usuario> CadastrarAsync(string nome, string email, string senha);
}
