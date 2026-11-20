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
        var idUsuario = IdUsuarioLogado();
        var pedido = new Pedido();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        // As leituras também ficam dentro da transação: assim o ListarPorIdsAsync usa FOR UPDATE
        // e trava o estoque desses jogos até o fim, e outra compra não lê o estoque antigo.
        // Toda conferência acontece antes da primeira gravação; sem o Complete(), nada fica no banco.
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(i => i.IdJogo).ToList());

            // Jogo que não veio (não existe) ou veio inativo: não está à venda.
            var jogosDoPedido = itens
                .Select(item => (Item: item, Jogo: jogos.SingleOrDefault(j => j.Id == item.IdJogo)))
                .ToList();
            if (jogosDoPedido.Any(x => x.Jogo is null || !x.Jogo.Ativo))
            {
                throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);
            }

            if (jogosDoPedido.Any(x => x.Jogo!.Estoque < x.Item.Quantidade))
            {
                throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);
            }

            // O preço vem do banco, nunca do front: pelo F12 o cliente poderia mandar outro valor.
            pedido.Data = DateTime.UtcNow;
            pedido.Total = jogosDoPedido.Sum(x => x.Jogo!.Preco * x.Item.Quantidade);
            pedido.IdUsuario = idUsuario;
            pedido.IdSituacaoPedido = SituacaoPedidoIds.Pendente;
            pedido.Id = await repositorio.Pedido.InserirAsync(pedido);

            foreach (var (item, jogo) in jogosDoPedido)
            {
                // vl_unitario guarda o preço pago: se o preço do jogo mudar depois, o pedido não muda.
                await repositorio.ItemPedido.InserirAsync(new ItemPedido
                {
                    IdPedido = pedido.Id,
                    IdJogo = jogo!.Id,
                    Quantidade = item.Quantidade,
                    ValorUnitario = jogo.Preco,
                });
                await repositorio.Jogo.AlterarEstoqueAsync(jogo.Id, jogo.Estoque - item.Quantidade);
            }

            transactionScope.Complete();
        }

        return (await MontarDetalhesAsync(new List<Pedido> { pedido })).Single();
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarMeusAsync()
    {
        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(IdUsuarioLogado()); // já vem ordenado no SQL
        return await MontarDetalhesAsync(pedidos);
    }

    public async Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var pedido = await BuscarPedidoAsync(id);

        // 403 por regra de negócio, não por perfil: a rota é permitida a qualquer usuário logado,
        // mas um cliente só pode ver o pedido que é dele.
        if (!usuarioAtual.EhAdministrador && pedido.IdUsuario != usuarioAtual.IdUsuario)
        {
            throw new BusinessRuleException(BusinessErrors.PEDIDO_DE_OUTRO_USUARIO);
        }

        return (await MontarDetalhesAsync(new List<Pedido> { pedido })).Single();
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarAsync(int? idSituacao)
    {
        var pedidos = await repositorio.Pedido.ListarAsync(idSituacao); // já vem ordenado no SQL
        return await MontarDetalhesAsync(pedidos);
    }

    public async Task<PedidoDetalheServiceModel> AlterarSituacaoAsync(int id, int idSituacao)
    {
        var pedido = await BuscarPedidoAsync(id);

        _ = await repositorio.SituacaoPedido.BuscarPorIdAsync(idSituacao)
            ?? throw new BusinessRuleException(BusinessErrors.SITUACAO_NAO_ENCONTRADA);

        // Um único comando: não precisa de transação. O desafio não pede bloquear
        // transições nem devolver estoque ao cancelar, então nenhuma regra extra aqui.
        await repositorio.Pedido.AlterarSituacaoAsync(id, idSituacao);
        pedido.IdSituacaoPedido = idSituacao;

        return (await MontarDetalhesAsync(new List<Pedido> { pedido })).Single();
    }

    // As rotas de pedido exigem login (política padrão), então sempre há um usuário aqui.
    private int IdUsuarioLogado()
    {
        return usuarioAtual.IdUsuario
            ?? throw new InvalidOperationException("Operação de pedido chamada sem usuário logado.");
    }

    private async Task<Pedido> BuscarPedidoAsync(int id)
    {
        return await repositorio.Pedido.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.PEDIDO_NAO_ENCONTRADO);
    }

    // Junta pedido + itens + jogos + situação + usuário, que estão em tabelas diferentes.
    // São sempre 4 consultas (uma por tabela), qualquer que seja o número de pedidos;
    // a combinação é feita em memória com LINQ, igual ao JogoService.
    private async Task<List<PedidoDetalheServiceModel>> MontarDetalhesAsync(List<Pedido> pedidos)
    {
        if (pedidos.Count == 0)
        {
            return new List<PedidoDetalheServiceModel>();
        }

        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(pedidos.Select(p => p.Id).ToList());

        // ListarPorIdsAsync traz também os jogos inativos: um pedido antigo pode ter jogo desativado.
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(i => i.IdJogo).Distinct().ToList());
        var situacoes = await repositorio.SituacaoPedido.ListarAsync();
        var usuarios = await repositorio.Usuario.ListarPorIdsAsync(pedidos.Select(p => p.IdUsuario).Distinct().ToList());

        var detalhes =
            from pedido in pedidos
            join situacao in situacoes on pedido.IdSituacaoPedido equals situacao.Id
            join usuario in usuarios on pedido.IdUsuario equals usuario.Id
            // "into" agrupa os itens de cada pedido (um pedido tem vários itens)
            join item in itens on pedido.Id equals item.IdPedido into itensDoPedido
            select new PedidoDetalheServiceModel
            {
                Id = pedido.Id,
                Data = pedido.Data,
                IdSituacao = situacao.Id,
                DescricaoSituacao = situacao.Descricao,
                Total = pedido.Total,
                IdUsuario = usuario.Id,
                NomeUsuario = usuario.Nome,
                Itens = (
                    from item in itensDoPedido
                    join jogo in jogos on item.IdJogo equals jogo.Id
                    orderby jogo.Nome
                    select new ItemPedidoDetalheServiceModel
                    {
                        IdJogo = jogo.Id,
                        NomeJogo = jogo.Nome,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                    }
                ).ToList(),
            };

        return detalhes.ToList();
    }
}
