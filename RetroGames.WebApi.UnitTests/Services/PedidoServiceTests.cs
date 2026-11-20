using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private const int IdUsuarioLogado = 7;
    private const int IdPedido = 12;
    private readonly Mock<IPedidoRepository> pedidos = new();
    private readonly Mock<IItemPedidoRepository> itens = new();
    private readonly Mock<IJogoRepository> jogos = new();
    private readonly Mock<ISituacaoPedidoRepository> situacoes = new();
    private readonly Mock<IUsuarioRepository> usuarios = new();
    private readonly Mock<IRepositorySet> repositorio = new();
    private readonly Mock<IUsuarioAtualService> usuarioAtual = new();

    private PedidoService CriarService()
    {
        repositorio.SetupGet(repo => repo.Pedido).Returns(pedidos.Object);
        repositorio.SetupGet(repo => repo.ItemPedido).Returns(itens.Object);
        repositorio.SetupGet(repo => repo.Jogo).Returns(jogos.Object);
        repositorio.SetupGet(repo => repo.SituacaoPedido).Returns(situacoes.Object);
        repositorio.SetupGet(repo => repo.Usuario).Returns(usuarios.Object);
        usuarioAtual.SetupGet(usuario => usuario.IdUsuario).Returns(IdUsuarioLogado);
        usuarioAtual.SetupGet(usuario => usuario.EstaAutenticado).Returns(true);
        return new PedidoService(repositorio.Object, usuarioAtual.Object);
    }

    private void PrepararMontagem(int idUsuario)
    {
        situacoes.Setup(repo => repo.ListarAsync()).ReturnsAsync([
            new SituacaoPedido { Id = SituacaoPedidoIds.Pendente, Descricao = "Pendente" },
            new SituacaoPedido { Id = SituacaoPedidoIds.Pago, Descricao = "Pago" },
        ]);
        usuarios.Setup(repo => repo.ListarPorIdsAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 1 && ids.Contains(idUsuario))))
            .ReturnsAsync([new Usuario { Id = idUsuario, Nome = "Maria" }]);
        itens.Setup(repo => repo.ListarPorPedidosAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 1 && ids.Contains(IdPedido))))
            .ReturnsAsync([]);
        jogos.Setup(repo => repo.ListarPorIdsAsync(It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 0)))
            .ReturnsAsync([]);
    }

    private void PrepararCompraValida()
    {
        PrepararMontagem(IdUsuarioLogado);
        jogos.Setup(repo => repo.ListarPorIdsAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 2 && ids.Contains(1) && ids.Contains(2))))
            .ReturnsAsync([
                new Jogo { Id = 1, Nome = "Aventura", Preco = 100m, Estoque = 5, Ativo = true },
                new Jogo { Id = 2, Nome = "Corrida", Preco = 50m, Estoque = 4, Ativo = true },
            ]);
        pedidos.Setup(repo => repo.InserirAsync(It.IsAny<Pedido>())).ReturnsAsync(IdPedido);
    }

    private void VerificarQueCompraNaoFoiGravada()
    {
        pedidos.Verify(repo => repo.InserirAsync(It.IsAny<Pedido>()), Times.Never);
        itens.Verify(repo => repo.InserirAsync(It.IsAny<ItemPedido>()), Times.Never);
        jogos.Verify(repo => repo.AlterarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_JogoInexistente_LancaJogoIndisponivel()
    {
        // Arrange
        jogos.Setup(repo => repo.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>())).ReturnsAsync([]);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync([(1, 2)]));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, excecao.Erro);
        Assert.Equal(110, excecao.Erro.Code);
        VerificarQueCompraNaoFoiGravada();
    }

    [Fact]
    public async Task Inserir_JogoInativo_LancaJogoIndisponivel()
    {
        // Arrange
        jogos.Setup(repo => repo.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>())).ReturnsAsync([new Jogo { Id = 1, Estoque = 5, Ativo = false }]);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync([(1, 2)]));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, excecao.Erro);
        Assert.Equal(110, excecao.Erro.Code);
        VerificarQueCompraNaoFoiGravada();
    }

    [Fact]
    public async Task Inserir_EstoqueInsuficiente_LancaEstoqueInsuficiente()
    {
        // Arrange
        jogos.Setup(repo => repo.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>())).ReturnsAsync([
            new Jogo { Id = 1, Estoque = 5, Ativo = true },
            new Jogo { Id = 2, Estoque = 0, Ativo = true },
        ]);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync([(1, 2), (2, 1)]));

        // Assert
        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE, excecao.Erro);
        Assert.Equal(111, excecao.Erro.Code);
        VerificarQueCompraNaoFoiGravada();
    }

    [Fact]
    public async Task Inserir_PedidoValido_CalculaTotalComPrecoDoBanco()
    {
        // Arrange
        PrepararCompraValida();
        decimal? totalGravado = null;
        pedidos.Setup(repo => repo.InserirAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido => totalGravado = pedido.Total)
            .ReturnsAsync(IdPedido);
        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync([(1, 2), (2, 1)]);

        // Assert
        Assert.Equal(250m, totalGravado);
        Assert.Equal(IdPedido, resultado.Id);
        pedidos.Verify(repo => repo.InserirAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task Inserir_PedidoValido_GravaPendenteComDataUtc()
    {
        // Arrange
        PrepararCompraValida();
        (int Situacao, DateTime Data)? valoresGravados = null;
        pedidos.Setup(repo => repo.InserirAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido => valoresGravados = (pedido.IdSituacaoPedido, pedido.Data))
            .ReturnsAsync(IdPedido);
        var service = CriarService();
        var antesDaCompra = DateTime.UtcNow;

        // Act
        var resultado = await service.InserirAsync([(1, 2), (2, 1)]);

        // Assert
        Assert.NotNull(valoresGravados);
        Assert.Equal(SituacaoPedidoIds.Pendente, valoresGravados.Value.Situacao);
        Assert.Equal(DateTimeKind.Utc, valoresGravados.Value.Data.Kind);
        Assert.InRange(valoresGravados.Value.Data, antesDaCompra, DateTime.UtcNow);
        Assert.Equal(IdPedido, resultado.Id);
        pedidos.Verify(repo => repo.InserirAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task Inserir_PedidoValido_UsaUsuarioLogado()
    {
        // Arrange
        PrepararCompraValida();
        int? usuarioGravado = null;
        pedidos.Setup(repo => repo.InserirAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido => usuarioGravado = pedido.IdUsuario)
            .ReturnsAsync(IdPedido);
        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync([(1, 2), (2, 1)]);

        // Assert
        Assert.Equal(IdUsuarioLogado, usuarioGravado);
        Assert.Equal(IdPedido, resultado.Id);
        pedidos.Verify(repo => repo.InserirAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task Inserir_PedidoValido_GravaPrecoUnitarioDoBanco()
    {
        // Arrange
        PrepararCompraValida();
        var itensGravados = new List<(int IdPedido, int IdJogo, int Quantidade, decimal ValorUnitario)>();
        itens.Setup(repo => repo.InserirAsync(It.IsAny<ItemPedido>()))
            .Callback<ItemPedido>(item => itensGravados.Add(
                (item.IdPedido, item.IdJogo, item.Quantidade, item.ValorUnitario)))
            .Returns(Task.CompletedTask);
        var service = CriarService();

        // Act
        await service.InserirAsync([(1, 2), (2, 1)]);

        // Assert
        Assert.Equal(
            [(IdPedido, 1, 2, 100m), (IdPedido, 2, 1, 50m)],
            itensGravados.OrderBy(item => item.IdJogo).ToArray());
        itens.Verify(repo => repo.InserirAsync(It.IsAny<ItemPedido>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Inserir_PedidoValido_BaixaEstoque()
    {
        // Arrange
        PrepararCompraValida();
        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync([(1, 2), (2, 1)]);

        // Assert
        Assert.Equal(IdPedido, resultado.Id);
        jogos.Verify(repo => repo.AlterarEstoqueAsync(1, 3), Times.Once);
        jogos.Verify(repo => repo.AlterarEstoqueAsync(2, 3), Times.Once);
        jogos.Verify(repo => repo.AlterarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ListarMeus_ComPedidos_ConsultaSoOUsuarioLogado()
    {
        // Arrange
        var pedido = new Pedido
        {
            Id = IdPedido,
            IdUsuario = IdUsuarioLogado,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente,
        };
        pedidos.Setup(repo => repo.ListarPorUsuarioAsync(IdUsuarioLogado)).ReturnsAsync([pedido]);
        PrepararMontagem(IdUsuarioLogado);
        var service = CriarService();

        // Act
        var resultado = await service.ListarMeusAsync();

        // Assert
        var detalhe = Assert.Single(resultado);
        Assert.Equal(IdPedido, detalhe.Id);
        Assert.Equal(IdUsuarioLogado, detalhe.IdUsuario);
        pedidos.Verify(repo => repo.ListarPorUsuarioAsync(IdUsuarioLogado), Times.Once);
        pedidos.Verify(repo => repo.ListarPorUsuarioAsync(It.Is<int>(id => id != IdUsuarioLogado)), Times.Never);
        pedidos.Verify(repo => repo.ListarAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task BuscarPorId_Inexistente_LancaPedidoNaoEncontrado()
    {
        // Arrange
        pedidos.Setup(repo => repo.BuscarPorIdAsync(IdPedido)).ReturnsAsync((Pedido?)null);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(IdPedido));

        // Assert
        Assert.Equal(BusinessErrors.PEDIDO_NAO_ENCONTRADO, excecao.Erro);
        Assert.Equal(109, excecao.Erro.Code);
        pedidos.Verify(repo => repo.BuscarPorIdAsync(IdPedido), Times.Once);
    }

    [Fact]
    public async Task BuscarPorId_PedidoDeOutroCliente_LancaPedidoDeOutroUsuario()
    {
        // Arrange
        pedidos.Setup(repo => repo.BuscarPorIdAsync(IdPedido)).ReturnsAsync(new Pedido
        {
            Id = IdPedido,
            IdUsuario = 8,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente,
        });
        usuarioAtual.SetupGet(usuario => usuario.EhAdministrador).Returns(false);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(IdPedido));

        // Assert
        Assert.Equal(BusinessErrors.PEDIDO_DE_OUTRO_USUARIO, excecao.Erro);
        Assert.Equal(113, excecao.Erro.Code);
        itens.Verify(repo => repo.ListarPorPedidosAsync(It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
    }

    [Fact]
    public async Task BuscarPorId_AdministradorPedidoDeOutro_DevolvePedido()
    {
        // Arrange
        const int idDono = 8;
        pedidos.Setup(repo => repo.BuscarPorIdAsync(IdPedido)).ReturnsAsync(new Pedido
        {
            Id = IdPedido,
            IdUsuario = idDono,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente,
        });
        usuarioAtual.SetupGet(usuario => usuario.EhAdministrador).Returns(true);
        PrepararMontagem(idDono);
        var service = CriarService();

        // Act
        var resultado = await service.BuscarPorIdAsync(IdPedido);

        // Assert
        Assert.Equal(IdPedido, resultado.Id);
        Assert.Equal(idDono, resultado.IdUsuario);
        pedidos.Verify(repo => repo.BuscarPorIdAsync(IdPedido), Times.Once);
    }
}
