using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class GeneroMapper
{
    public static ListarGeneroResponseApiModel ParaResponse(this Genero genero) => new()
    {
        Id = genero.Id,
        Nome = genero.Nome,
    };
}
