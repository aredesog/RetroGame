using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Erros;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    private readonly Mock<IGeneroRepository> generos = new();
    private readonly Mock<IJogoGeneroRepository> jogosGeneros = new();
    private readonly Mock<IRepositorySet> repositorio = new();

    private GeneroService CriarService()
    {
        repositorio.SetupGet(repo => repo.Genero).Returns(generos.Object);
        repositorio.SetupGet(repo => repo.JogoGenero).Returns(jogosGeneros.Object);
        return new GeneroService(repositorio.Object);
    }

    [Fact]
    public async Task Listar_RepositorioComRegistros_DevolveRegistros()
    {
        var esperados = new List<Genero>
        {
            new() { Id = 1, Nome = "Aventura" },
            new() { Id = 2, Nome = "Corrida" },
        };
        var generos = new Mock<IGeneroRepository>();
        generos.Setup(repo => repo.ListarAsync()).ReturnsAsync(esperados);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(repo => repo.Genero).Returns(generos.Object);
        var service = new GeneroService(repositorio.Object);

        var resultado = await service.ListarAsync();

        Assert.Equal(esperados, resultado);
        generos.Verify(repo => repo.ListarAsync(), Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var genero = new Genero { Nome = "RPG" };
        generos.Setup(repo => repo.ExisteComNomeAsync("RPG", null)).ReturnsAsync(true);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(genero));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NOME_DUPLICADO, excecao.Erro);
        Assert.Equal(107, excecao.Erro.Code);
        generos.Verify(repo => repo.InserirAsync(It.IsAny<Genero>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_RepositorioLancaRegistroDuplicado_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var genero = new Genero { Nome = "RPG" };
        generos.Setup(repo => repo.ExisteComNomeAsync("RPG", null)).ReturnsAsync(false);
        generos.Setup(repo => repo.InserirAsync(genero))
            .ThrowsAsync(new RegistroDuplicadoException("Nome duplicado", new InvalidOperationException()));
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(genero));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NOME_DUPLICADO, excecao.Erro);
        Assert.Equal(107, excecao.Erro.Code);
        generos.Verify(repo => repo.InserirAsync(genero), Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DevolveGeneroComId()
    {
        // Arrange
        const int idGerado = 7;
        var genero = new Genero { Nome = "RPG" };
        generos.Setup(repo => repo.ExisteComNomeAsync("RPG", null)).ReturnsAsync(false);
        generos.Setup(repo => repo.InserirAsync(genero)).ReturnsAsync(idGerado);
        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync(genero);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        Assert.Equal("RPG", resultado.Nome);
        generos.Verify(repo => repo.InserirAsync(genero), Times.Once);
    }

    [Fact]
    public async Task Alterar_MantendoProprioNome_Grava()
    {
        // Arrange
        var genero = new Genero { Id = 7, Nome = "RPG" };
        generos.Setup(repo => repo.BuscarPorIdAsync(7)).ReturnsAsync(new Genero { Id = 7, Nome = "RPG" });
        generos.Setup(repo => repo.ExisteComNomeAsync("RPG", It.IsAny<int?>())).ReturnsAsync(true);
        generos.Setup(repo => repo.ExisteComNomeAsync("RPG", 7)).ReturnsAsync(false);
        var service = CriarService();

        // Act
        var resultado = await service.AlterarAsync(genero);

        // Assert
        Assert.Equal(7, resultado.Id);
        Assert.Equal("RPG", resultado.Nome);
        generos.Verify(repo => repo.ExisteComNomeAsync("RPG", 7), Times.Once);
        generos.Verify(repo => repo.AlterarAsync(genero), Times.Once);
    }

    [Fact]
    public async Task Alterar_Inexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        var genero = new Genero { Id = 7, Nome = "RPG" };
        generos.Setup(repo => repo.BuscarPorIdAsync(7)).ReturnsAsync((Genero?)null);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.AlterarAsync(genero));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, excecao.Erro);
        Assert.Equal(105, excecao.Erro.Code);
        generos.Verify(repo => repo.AlterarAsync(It.IsAny<Genero>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_GeneroComJogos_LancaGeneroPossuiJogos()
    {
        // Arrange
        const int id = 7;
        generos.Setup(repo => repo.BuscarPorIdAsync(id)).ReturnsAsync(new Genero { Id = id });
        jogosGeneros.Setup(repo => repo.ExisteJogoComGeneroAsync(id)).ReturnsAsync(true);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.ExcluirAsync(id));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_POSSUI_JOGOS, excecao.Erro);
        Assert.Equal(108, excecao.Erro.Code);
        generos.Verify(repo => repo.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_GeneroSemJogos_Exclui()
    {
        // Arrange
        const int id = 7;
        generos.Setup(repo => repo.BuscarPorIdAsync(id)).ReturnsAsync(new Genero { Id = id });
        jogosGeneros.Setup(repo => repo.ExisteJogoComGeneroAsync(id)).ReturnsAsync(false);
        var service = CriarService();

        // Act
        await service.ExcluirAsync(id);

        // Assert
        jogosGeneros.Verify(repo => repo.ExisteJogoComGeneroAsync(id), Times.Once);
        generos.Verify(repo => repo.ExcluirAsync(id), Times.Once);
    }
}
