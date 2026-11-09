using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

/// <summary>Service da tabela tb_condicao_jogo. Só leitura.</summary>
public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet repositorio;

    public CondicaoJogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return repositorio.CondicaoJogo.ListarAsync();
    }
}
