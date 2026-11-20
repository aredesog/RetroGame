using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Erros;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> plataformas = new();
    private readonly Mock<IRepositorySet> repositorio = new();

    private PlataformaService CriarService()
    {
        repositorio.SetupGet(conjunto => conjunto.Plataforma).Returns(plataformas.Object);
        return new PlataformaService(repositorio.Object);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        const int id = 42;
        plataformas.Setup(repo => repo.BuscarPorIdAsync(id)).ReturnsAsync((Plataforma?)null);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(id));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, excecao.Erro);
    }

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicadoSemInserir()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "PlayStation" };
        plataformas.Setup(repo => repo.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(true);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, excecao.Erro);
        plataformas.Verify(repo => repo.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_RepositorioRecusaDuplicata_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "PlayStation" };
        plataformas.Setup(repo => repo.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(false);
        plataformas.Setup(repo => repo.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException("Nome duplicado", new InvalidOperationException()));
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, excecao.Erro);
        plataformas.Verify(repo => repo.InserirAsync(It.IsAny<Plataforma>()), Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DevolvePlataformaComIdGerado()
    {
        // Arrange
        const int idGerado = 7;
        var plataforma = new Plataforma { Nome = "Dreamcast" };
        plataformas.Setup(repo => repo.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(false);
        plataformas.Setup(repo => repo.InserirAsync(plataforma)).ReturnsAsync(idGerado);
        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync(plataforma);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        Assert.Equal("Dreamcast", resultado.Nome);
        plataformas.Verify(repo => repo.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Alterar_MantemProprioNome_GravaPlataforma()
    {
        // Arrange
        var plataforma = new Plataforma { Id = 7, Nome = "Dreamcast", Fabricante = "Sega" };
        plataformas.Setup(repo => repo.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(new Plataforma { Id = plataforma.Id, Nome = plataforma.Nome });
        plataformas.Setup(repo => repo.ExisteComNomeAsync(plataforma.Nome, It.IsAny<int?>())).ReturnsAsync(true);
        plataformas.Setup(repo => repo.ExisteComNomeAsync(plataforma.Nome, plataforma.Id)).ReturnsAsync(false);
        var service = CriarService();

        // Act
        await service.AlterarAsync(plataforma);

        // Assert
        plataformas.Verify(repo => repo.ExisteComNomeAsync("Dreamcast", 7), Times.Once);
        plataformas.Verify(repo => repo.AlterarAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogosSemExcluir()
    {
        // Arrange
        const int id = 7;
        plataformas.Setup(repo => repo.BuscarPorIdAsync(id)).ReturnsAsync(new Plataforma { Id = id });
        plataformas.Setup(repo => repo.PossuiJogosAsync(id)).ReturnsAsync(true);
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(() => service.ExcluirAsync(id));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, excecao.Erro);
        plataformas.Verify(repo => repo.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
