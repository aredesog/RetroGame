using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

// C07: tabela de domínio (1 Pendente, 2 Pago, 3 Enviado, 4 Cancelado), só leitura.
// Usada pela tela de gestão de pedidos para montar o filtro e a troca de situação.
public class SituacaoPedidoService : ISituacaoPedidoService
{
    private readonly IRepositorySet repositorio;

    public SituacaoPedidoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<SituacaoPedido>> ListarAsync()
    {
        return repositorio.SituacaoPedido.ListarAsync();
    }
}
