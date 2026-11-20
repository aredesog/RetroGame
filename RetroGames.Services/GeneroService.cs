using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

// C08: mesmas regras da manutenção de plataformas, com os códigos 105, 107 e 108.
public class GeneroService : IGeneroService
{
    private readonly IRepositorySet repositorio;

    public GeneroService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<Genero>> ListarAsync()
    {
        return repositorio.Genero.ListarAsync();
    }

    public async Task<Genero> BuscarPorIdAsync(int id)
    {
        return await repositorio.Genero.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
    }

    public async Task<Genero> InserirAsync(Genero genero)
    {
        genero.Nome = genero.Nome.Trim();

        if (await repositorio.Genero.ExisteComNomeAsync(genero.Nome))
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_NOME_DUPLICADO);
        }

        try
        {
            genero.Id = await repositorio.Genero.InserirAsync(genero);
        }
        catch (RegistroDuplicadoException)
        {
            // Duas requisições ao mesmo tempo passaram pela conferência acima;
            // o UNIQUE de no_genero barrou a segunda (erro 1062). Mesmo erro de negócio.
            throw new BusinessRuleException(BusinessErrors.GENERO_NOME_DUPLICADO);
        }

        return genero;
    }

    public async Task<Genero> AlterarAsync(Genero genero)
    {
        await BuscarPorIdAsync(genero.Id); // lança GENERO_NAO_ENCONTRADO se não existir

        genero.Nome = genero.Nome.Trim();

        // Passa o próprio id para ser ignorado: manter o mesmo nome não é duplicidade.
        if (await repositorio.Genero.ExisteComNomeAsync(genero.Nome, genero.Id))
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_NOME_DUPLICADO);
        }

        try
        {
            await repositorio.Genero.AlterarAsync(genero);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_NOME_DUPLICADO);
        }

        return genero;
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id); // lança GENERO_NAO_ENCONTRADO se não existir

        // Jogo inativo também conta: o vínculo continua em tb_jogo_genero (RN03).
        if (await repositorio.JogoGenero.ExisteJogoComGeneroAsync(id))
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_POSSUI_JOGOS);
        }

        await repositorio.Genero.ExcluirAsync(id);
    }
}
