using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

/// <summary>Regras de negócio de plataformas (RN01 e RN02).</summary>
public interface IPlataformaService
{
    Task<List<Plataforma>> ListarAsync();

    /// <summary>Lança PLATAFORMA_NAO_ENCONTRADA se não existir.</summary>
    Task<Plataforma> BuscarPorIdAsync(int id);

    /// <summary>RN01: lança PLATAFORMA_NOME_DUPLICADO se o nome já existir.</summary>
    Task<Plataforma> InserirAsync(Plataforma plataforma);

    /// <summary>Lança PLATAFORMA_NAO_ENCONTRADA ou PLATAFORMA_NOME_DUPLICADO.</summary>
    Task<Plataforma> AlterarAsync(Plataforma plataforma);

    /// <summary>RN02: lança PLATAFORMA_NAO_ENCONTRADA ou PLATAFORMA_POSSUI_JOGOS.</summary>
    Task ExcluirAsync(int id);
}
