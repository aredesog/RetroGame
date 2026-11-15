using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class CondicaoJogoMapper
{
    public static ListarCondicaoJogoResponseApiModel ParaResponse(this CondicaoJogo condicao) => new()
    {
        Id = condicao.Id,
        Descricao = condicao.Descricao,
    };
}
