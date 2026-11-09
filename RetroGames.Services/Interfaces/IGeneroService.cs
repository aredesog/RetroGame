using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

/// <summary>Leitura de gêneros (sem regra de negócio).</summary>
public interface IGeneroService
{
    Task<List<Genero>> ListarAsync();
}
