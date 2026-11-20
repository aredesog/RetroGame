using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_RepositorioComRegistros_DevolveRegistros()
    {
        var esperados = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Usado" },
        };
        var condicoes = new Mock<ICondicaoJogoRepository>();
        condicoes.Setup(repo => repo.ListarAsync()).ReturnsAsync(esperados);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(repo => repo.CondicaoJogo).Returns(condicoes.Object);
        var service = new CondicaoJogoService(repositorio.Object);

        var resultado = await service.ListarAsync();

        Assert.Equal(esperados, resultado);
        condicoes.Verify(repo => repo.ListarAsync(), Times.Once);
    }
}
