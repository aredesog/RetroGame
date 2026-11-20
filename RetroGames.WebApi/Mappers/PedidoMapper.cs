using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PedidoMapper
{
    public static ListarPedidoResponseApiModel ParaResponse(this PedidoDetalheServiceModel pedido) => new()
    {
        Id = pedido.Id,
        Data = pedido.Data,
        IdSituacao = pedido.IdSituacao,
        DescricaoSituacao = pedido.DescricaoSituacao,
        Total = pedido.Total,
        IdUsuario = pedido.IdUsuario,
        NomeUsuario = pedido.NomeUsuario,
        Itens = pedido.Itens.Select(i => new ListarItemPedidoResponseApiModel
        {
            IdJogo = i.IdJogo,
            NomeJogo = i.NomeJogo,
            Quantidade = i.Quantidade,
            ValorUnitario = i.ValorUnitario,
        }).ToList(),
    };
}
