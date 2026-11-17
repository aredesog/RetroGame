using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet repositorio;
    private readonly IPasswordHasher<Usuario> passwordHasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> passwordHasher)
    {
        this.repositorio = repositorio;
        this.passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        // A senha não é "descriptografada": o hasher gera o hash da senha digitada,
        // com o mesmo salt guardado dentro do hash, e compara os dois.
        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        if (await repositorio.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador,
        };
        administrador.SenhaHash = passwordHasher.HashPassword(administrador, senha); // RS04: só o hash vai ao banco

        await repositorio.Usuario.InserirAsync(administrador);
    }
}
