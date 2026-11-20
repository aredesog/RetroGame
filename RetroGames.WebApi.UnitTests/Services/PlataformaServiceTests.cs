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
}
