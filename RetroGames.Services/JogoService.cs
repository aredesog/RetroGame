using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

/// <summary>
/// Regras de negócio de jogos e montagem do catálogo com LINQ.
/// Não conhece SQL nem HTTP: usa só o IRepositorySet e o catálogo de erros.
/// </summary>
public class JogoService : IJogoService
{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public async Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro)
    {
        // Filtro por gênero: o Service seleciona com LINQ os ids dos jogos do gênero
        // e entrega a lista ao Repository, que continua paginando no SQL.
        List<int>? idsDoGenero = null;
        if (filtro.IdGenero is not null)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();
            idsDoGenero = vinculos
                .Where(v => v.IdGenero == filtro.IdGenero)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            filtro.Busca,
            filtro.IdPlataforma,
            idsDoGenero,
            incluirInativos: false, // administrador vendo inativos: aula 6
            filtro.Pagina,
            filtro.TamanhoPagina);

        return new PaginaServiceModel<JogoDetalheServiceModel>
        {
            Itens = await MontarDetalhesAsync(jogos),
            Total = total,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
        };
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        return (await MontarDetalhesAsync(new List<Jogo> { jogo })).Single();
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;
        var generos = idsGeneros.Distinct().ToList();

        await ConferirReferenciasAsync(jogo, generos); // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        _ = await repositorio.Jogo.BuscarPorIdAsync(jogo.Id)
            ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        jogo.Nome = jogo.Nome.Trim();
        var generos = idsGeneros.Distinct().ToList();

        await ConferirReferenciasAsync(jogo, generos); // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task DesativarAsync(int id)
    {
        _ = await repositorio.Jogo.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        await repositorio.Jogo.DesativarAsync(id); // RN03: nunca apaga
    }

    /// <summary>
    /// RN05: plataforma, condição e todos os gêneros precisam existir.
    /// </summary>
    private async Task ConferirReferenciasAsync(Jogo jogo, List<int> idsGeneros)
    {
        if (await repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CONDICAO_NAO_ENCONTRADA);
        }

        var idsExistentes = (await repositorio.Genero.ListarAsync()).Select(g => g.Id).ToHashSet();
        if (!idsGeneros.All(idsExistentes.Contains))
        {
            throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    /// <summary>
    /// Monta o detalhe de uma lista de jogos com LINQ.
    /// Faz sempre 4 consultas (plataformas, condições, gêneros e vínculos),
    /// qualquer que seja o tamanho da lista: nunca uma consulta por jogo.
    /// </summary>
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());

        var detalhes =
            from jogo in jogos
            join plataforma in plataformas on jogo.IdPlataforma equals plataforma.Id
            join condicao in condicoes on jogo.IdCondicaoJogo equals condicao.Id
            join vinculo in vinculos on jogo.Id equals vinculo.IdJogo into vinculosDoJogo
            select new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataforma.Nome,
                DescricaoCondicao = condicao.Descricao,
                Generos = (
                    from vinculo in vinculosDoJogo
                    join genero in generos on vinculo.IdGenero equals genero.Id
                    orderby genero.Nome
                    select genero
                ).ToList(),
            };

        return detalhes.ToList();
    }
}
