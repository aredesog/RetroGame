using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IPedidoService
{
    // C04: grava pedido, itens e baixa de estoque numa única transação.
    // Jogo inexistente ou inativo → 110; estoque insuficiente → 111 (e nada é gravado).
    Task<PedidoDetalheServiceModel> InserirAsync(IReadOnlyCollection<(int IdJogo, int Quantidade)> itens);

    // C05: pedidos do usuário logado, do mais recente para o mais antigo.
    Task<List<PedidoDetalheServiceModel>> ListarMeusAsync();

    // C06: o cliente só vê os próprios pedidos (113); o administrador vê qualquer um.
    Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int id);

    // C07: todos os pedidos, do mais recente para o mais antigo. idSituacao nulo = sem filtro.
    Task<List<PedidoDetalheServiceModel>> ListarAsync(int? idSituacao);

    // C07: pedido inexistente → 109; situação inexistente → 112.
    Task<PedidoDetalheServiceModel> AlterarSituacaoAsync(int id, int idSituacao);
}
