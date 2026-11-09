using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

/// <summary>Leitura das condições de jogo (sem regra de negócio).</summary>
public interface ICondicaoJogoService
{
    Task<List<CondicaoJogo>> ListarAsync();
}
