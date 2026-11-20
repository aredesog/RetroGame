using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
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
}
