using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

/// <summary>Service da tabela tb_genero. Só leitura.</summary>
public class GeneroService : IGeneroService
{
    private readonly IRepositorySet repositorio;

    public GeneroService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<Genero>> ListarAsync()
    {
        return repositorio.Genero.ListarAsync();
    }
}
