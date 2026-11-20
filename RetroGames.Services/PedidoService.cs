using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorySet repositorio;
    private readonly IUsuarioAtualService usuarioAtual;

    public PedidoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        this.repositorio = repositorio;
        this.usuarioAtual = usuarioAtual;
    }

    public async Task<PedidoDetalheServiceModel> InserirAsync(IReadOnlyCollection<(int IdJogo, int Quantidade)> itens)
    {
        var idUsuario = ObterIdUsuario();
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            var jogos = (await repositorio.Jogo.ListarPorIdsAsync(itens.Select(item => item.IdJogo).ToArray()))
                .ToDictionary(jogo => jogo.Id);
            if (itens.Any(item => !jogos.TryGetValue(item.IdJogo, out var jogo) || !jogo.Ativo))
            {
                throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);
            }

            if (itens.Any(item => jogos[item.IdJogo].Estoque < item.Quantidade))
            {
                throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);
            }

            var pedido = new Pedido
            {
                Data = DateTime.UtcNow,
                Total = itens.Sum(item => jogos[item.IdJogo].Preco * item.Quantidade),
                IdUsuario = idUsuario,
                IdSituacaoPedido = SituacaoPedidoIds.Pendente,
            };
            pedido.Id = await repositorio.Pedido.InserirAsync(pedido);

            foreach (var item in itens)
            {
                await repositorio.ItemPedido.InserirAsync(new ItemPedido
                {
                    IdPedido = pedido.Id,
                    IdJogo = item.IdJogo,
                    Quantidade = item.Quantidade,
                    ValorUnitario = jogos[item.IdJogo].Preco,
                });
            }

            foreach (var item in itens)
            {
                await repositorio.Jogo.AlterarEstoqueAsync(item.IdJogo, jogos[item.IdJogo].Estoque - item.Quantidade);
            }

            var detalhe = (await MontarDetalhesAsync([pedido])).Single();
            transactionScope.Complete();
            return detalhe;
        }
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarMeusAsync()
    {
        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(ObterIdUsuario());
        return await MontarDetalhesAsync(pedidos);
    }

    internal async Task<List<PedidoDetalheServiceModel>> MontarDetalhesAsync(List<Pedido> pedidos)
    {
        if (pedidos.Count == 0) return [];

        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(pedidos.Select(pedido => pedido.Id).ToArray());
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(item => item.IdJogo).Distinct().ToArray());
        var situacoes = await repositorio.SituacaoPedido.ListarAsync();
        var usuarios = usuarioAtual.EhAdministrador
            ? await repositorio.Usuario.ListarPorIdsAsync(pedidos.Select(pedido => pedido.IdUsuario).Distinct().ToArray())
            : [];

        var jogosPorId = jogos.ToDictionary(jogo => jogo.Id);
        var situacoesPorId = situacoes.ToDictionary(situacao => situacao.Id);
        var usuariosPorId = usuarios.ToDictionary(usuario => usuario.Id);
        var itensPorPedido = itens.ToLookup(item => item.IdPedido);

        return pedidos.Select(pedido => new PedidoDetalheServiceModel
        {
            Id = pedido.Id,
            Data = pedido.Data,
            IdSituacao = pedido.IdSituacaoPedido,
            DescricaoSituacao = situacoesPorId[pedido.IdSituacaoPedido].Descricao,
            Total = pedido.Total,
            IdUsuario = pedido.IdUsuario,
            NomeUsuario = usuariosPorId.GetValueOrDefault(pedido.IdUsuario)?.Nome,
            Itens = itensPorPedido[pedido.Id].Select(item => new ItemPedidoDetalheServiceModel
            {
                IdJogo = item.IdJogo,
                NomeJogo = jogosPorId[item.IdJogo].Nome,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
            }).ToList(),
        }).ToList();
    }

    private int ObterIdUsuario() =>
        usuarioAtual.IdUsuario ?? throw new InvalidOperationException("Usuário atual sem identificador.");
}
