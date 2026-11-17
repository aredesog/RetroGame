namespace RetroGames.Services.Interfaces;

// Quem está fazendo a requisição. O Service usa esta interface sem saber de HTTP ou cookie;
// quem implementa é a WebApi.
public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }
    int? IdUsuario { get; }
    bool EhAdministrador { get; }
}
