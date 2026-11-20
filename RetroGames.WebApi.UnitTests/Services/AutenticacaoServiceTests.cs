using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Erros;

namespace RetroGames.WebApi.UnitTests.Services;

public class AutenticacaoServiceTests
{
    private const string Nome = "Cliente de Teste";
    private const string Email = "cliente@example.test";
    private const string SenhaFicticia = "Senha-ficticia-de-teste-123";
    private readonly Mock<IUsuarioRepository> usuarios = new();
    private readonly Mock<IRepositorySet> repositorio = new();
    private readonly PasswordHasher<Usuario> hasher = new();

    private AutenticacaoService CriarService()
    {
        repositorio.SetupGet(repo => repo.Usuario).Returns(usuarios.Object);
        return new AutenticacaoService(repositorio.Object, hasher);
    }

    [Fact]
    public async Task Cadastrar_EmailJaCadastrado_LancaEmailJaCadastrado()
    {
        // Arrange
        usuarios.Setup(repo => repo.BuscarPorEmailAsync(Email)).ReturnsAsync(new Usuario { Email = Email });
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CadastrarAsync(Nome, Email, SenhaFicticia));

        // Assert
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO, excecao.Erro);
        Assert.Equal(106, excecao.Erro.Code);
        usuarios.Verify(repo => repo.InserirAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Cadastrar_RepositorioLancaRegistroDuplicado_LancaEmailJaCadastrado()
    {
        // Arrange
        usuarios.Setup(repo => repo.BuscarPorEmailAsync(Email)).ReturnsAsync((Usuario?)null);
        usuarios.Setup(repo => repo.InserirAsync(It.IsAny<Usuario>()))
            .ThrowsAsync(new RegistroDuplicadoException("E-mail duplicado", new InvalidOperationException()));
        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CadastrarAsync(Nome, Email, SenhaFicticia));

        // Assert
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO, excecao.Erro);
        Assert.Equal(106, excecao.Erro.Code);
        usuarios.Verify(repo => repo.InserirAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task Cadastrar_Valido_GravaClienteAtivoComSenhaEmHash()
    {
        // Arrange
        const int idGerado = 23;
        (string Nome, string Email, bool Ativo, int IdPerfil, string SenhaHash)? valoresGravados = null;
        usuarios.Setup(repo => repo.BuscarPorEmailAsync(Email)).ReturnsAsync((Usuario?)null);
        usuarios.Setup(repo => repo.InserirAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(usuario => valoresGravados =
                (usuario.Nome, usuario.Email, usuario.Ativo, usuario.IdPerfil, usuario.SenhaHash))
            .ReturnsAsync(idGerado);
        var service = CriarService();

        // Act
        var resultado = await service.CadastrarAsync(Nome, Email, SenhaFicticia);

        // Assert
        Assert.NotNull(valoresGravados);
        var gravado = valoresGravados.Value;
        Assert.Equal(Nome, gravado.Nome);
        Assert.Equal(Email, gravado.Email);
        Assert.True(gravado.Ativo);
        Assert.Equal(PerfilIds.Cliente, gravado.IdPerfil);
        Assert.NotEqual(SenhaFicticia, gravado.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success,
            hasher.VerifyHashedPassword(resultado, gravado.SenhaHash, SenhaFicticia));
        Assert.Equal(idGerado, resultado.Id);
        usuarios.Verify(repo => repo.InserirAsync(It.IsAny<Usuario>()), Times.Once);
    }
}
