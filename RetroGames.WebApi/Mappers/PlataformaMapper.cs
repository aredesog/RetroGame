using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(this Plataforma plataforma) => new()
    {
        Id = plataforma.Id,
        Nome = plataforma.Nome,
        Fabricante = plataforma.Fabricante,
        AnoLancamento = plataforma.AnoLancamento,
    };
}
