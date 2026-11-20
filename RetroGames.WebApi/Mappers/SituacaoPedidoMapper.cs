using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class SituacaoPedidoMapper
{
    public static ListarSituacaoPedidoResponseApiModel ParaResponse(this SituacaoPedido situacao) => new()
    {
        Id = situacao.Id,
        Descricao = situacao.Descricao,
    };
}
