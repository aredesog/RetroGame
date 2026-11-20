using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> jogos = new();
    private readonly Mock<IPlataformaRepository> plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> condicoes = new();
    private readonly Mock<IGeneroRepository> generos = new();
    private readonly Mock<IJogoGeneroRepository> vinculos = new();
    private readonly JogoService service;

    public JogoServiceTests()
    {
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(repo => repo.Jogo).Returns(jogos.Object);
        repositorio.SetupGet(repo => repo.Plataforma).Returns(plataformas.Object);
        repositorio.SetupGet(repo => repo.CondicaoJogo).Returns(condicoes.Object);
        repositorio.SetupGet(repo => repo.Genero).Returns(generos.Object);
        repositorio.SetupGet(repo => repo.JogoGenero).Returns(vinculos.Object);
        service = new JogoService(repositorio.Object, Mock.Of<IUsuarioAtualService>());
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaNaoEncontradaSemGravar()
    {
        var jogo = new Jogo { IdPlataforma = 1, IdCondicaoJogo = 2 };
        plataformas.Setup(repo => repo.BuscarPorIdAsync(1)).ReturnsAsync((Plataforma?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(jogo, [3]));

        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
        VerificarQueNaoGravou();
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaNaoEncontradaSemGravar()
    {
        var jogo = new Jogo { IdPlataforma = 1, IdCondicaoJogo = 2 };
        plataformas.Setup(repo => repo.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });
        condicoes.Setup(repo => repo.ListarAsync()).ReturnsAsync([new CondicaoJogo { Id = 4 }]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(jogo, [3]));

        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, erro.Erro);
        VerificarQueNaoGravou();
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaNaoEncontradoSemGravar()
    {
        var jogo = new Jogo { IdPlataforma = 1, IdCondicaoJogo = 2 };
        plataformas.Setup(repo => repo.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });
        condicoes.Setup(repo => repo.ListarAsync()).ReturnsAsync([new CondicaoJogo { Id = 2 }]);
        generos.Setup(repo => repo.ListarAsync()).ReturnsAsync([new Genero { Id = 3 }]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(jogo, [3, 4]));

        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, erro.Erro);
        VerificarQueNaoGravou();
    }

    [Fact]
    public async Task Listar_ComGenero_EnviaApenasIdsDosJogosVinculados()
    {
        vinculos.Setup(repo => repo.ListarTodosAsync()).ReturnsAsync([
            new JogoGenero { IdJogo = 10, IdGenero = 3 },
            new JogoGenero { IdJogo = 20, IdGenero = 4 },
            new JogoGenero { IdJogo = 30, IdGenero = 3 },
        ]);
        PrepararPaginaVazia();

        await service.ListarAsync(new JogoFiltroServiceModel { IdGenero = 3, Pagina = 2, TamanhoPagina = 5 });

        jogos.Verify(repo => repo.ListarAsync(null, null,
            It.Is<IReadOnlyCollection<int>>(ids => ids != null && ids.Count == 2 && ids.Contains(10) && ids.Contains(30)),
            false, 2, 5), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosNemRestringeIds()
    {
        PrepararPaginaVazia();

        await service.ListarAsync(new JogoFiltroServiceModel { Pagina = 1, TamanhoPagina = 5 });

        jogos.Verify(repo => repo.ListarAsync(null, null, null, false, 1, 5), Times.Once);
        vinculos.Verify(repo => repo.ListarTodosAsync(), Times.Never);
    }

    [Fact]
    public async Task Listar_PaginaComJogos_DevolveDetalhesDeCadaJogo()
    {
        var primeiro = new Jogo { Id = 10, IdPlataforma = 1, IdCondicaoJogo = 2 };
        var segundo = new Jogo { Id = 20, IdPlataforma = 4, IdCondicaoJogo = 5 };
        jogos.Setup(repo => repo.ListarAsync(null, null, null, false, 2, 2))
            .ReturnsAsync((new List<Jogo> { primeiro, segundo }, 4));
        plataformas.Setup(repo => repo.ListarAsync()).ReturnsAsync([
            new Plataforma { Id = 4, Nome = "Dreamcast" },
            new Plataforma { Id = 1, Nome = "PlayStation" },
        ]);
        condicoes.Setup(repo => repo.ListarAsync()).ReturnsAsync([
            new CondicaoJogo { Id = 5, Descricao = "Bom" },
            new CondicaoJogo { Id = 2, Descricao = "Novo" },
        ]);
        generos.Setup(repo => repo.ListarAsync()).ReturnsAsync([new Genero { Id = 3, Nome = "Corrida" }]);
        vinculos.Setup(repo => repo.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync([new JogoGenero { IdJogo = 20, IdGenero = 3 }]);

        var pagina = await service.ListarAsync(new JogoFiltroServiceModel { Pagina = 2, TamanhoPagina = 2 });

        Assert.Equal(4, pagina.Total);
        Assert.Equal(2, pagina.Pagina);
        Assert.Equal(2, pagina.TamanhoPagina);
        Assert.Collection(pagina.Itens, detalhe =>
        {
            Assert.Equal(10, detalhe.Jogo.Id);
            Assert.Equal("PlayStation", detalhe.NomePlataforma);
            Assert.Equal("Novo", detalhe.DescricaoCondicao);
            Assert.Empty(detalhe.Generos);
        }, detalhe =>
        {
            Assert.Equal(20, detalhe.Jogo.Id);
            Assert.Equal("Dreamcast", detalhe.NomePlataforma);
            Assert.Equal("Bom", detalhe.DescricaoCondicao);
            Assert.Equal("Corrida", Assert.Single(detalhe.Generos).Nome);
        });
        vinculos.Verify(repo => repo.ListarPorJogosAsync(
            It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 2 && ids.Contains(10) && ids.Contains(20))), Times.Once);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrado()
    {
        jogos.Setup(repo => repo.BuscarPorIdAsync(42)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(42));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task Desativar_IdInexistente_LancaNaoEncontradoSemDesativar()
    {
        jogos.Setup(repo => repo.BuscarPorIdAsync(42)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.DesativarAsync(42));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
        jogos.Verify(repo => repo.DesativarAsync(It.IsAny<int>()), Times.Never);
    }

    private void PrepararPaginaVazia()
    {
        jogos.Setup(repo => repo.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(),
            It.IsAny<IReadOnlyCollection<int>?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Jogo>(), 0));
    }

    private void VerificarQueNaoGravou()
    {
        jogos.Verify(repo => repo.InserirAsync(It.IsAny<Jogo>()), Times.Never);
        vinculos.Verify(repo => repo.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
    }
}
