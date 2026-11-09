using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.Erros;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public async Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro)
    {
        // O gênero está em outra tabela (tb_jogo_genero). O Service pega com LINQ os ids
        // dos jogos desse gênero e passa ao Repository, que filtra e pagina no SQL.
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
            incluirInativos: false, // ver inativos é só do administrador (aula 6)
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
        jogo.DataCadastro = DateTime.UtcNow; // datas sempre em UTC
        var generos = idsGeneros.Distinct().ToList(); // ignora gênero repetido na lista

        await ConferirReferenciasAsync(jogo, generos); // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        // Jogo e gêneros entram juntos: se qualquer gravação falhar, nada fica no banco.
        // Complete() só no fim; sem ele, ao sair do using, tudo é desfeito.
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id); // devolve já com plataforma, condição e gêneros
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        _ = await repositorio.Jogo.BuscarPorIdAsync(jogo.Id) // só confere se existe
            ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        jogo.Nome = jogo.Nome.Trim();
        var generos = idsGeneros.Distinct().ToList(); // ignora gênero repetido na lista

        await ConferirReferenciasAsync(jogo, generos); // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            // Troca os gêneros apagando os vínculos antigos e gravando os novos.
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id); // devolve já com plataforma, condição e gêneros
    }

    public async Task DesativarAsync(int id)
    {
        _ = await repositorio.Jogo.BuscarPorIdAsync(id) // só confere se existe
            ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        await repositorio.Jogo.DesativarAsync(id); // RN03: nunca apaga
    }

    // RN05: plataforma, condição e todos os gêneros informados precisam existir.
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

    // Junta jogo + plataforma + condição + gêneros, que estão em tabelas diferentes.
    // São sempre 4 consultas (uma por tabela), qualquer que seja o número de jogos;
    // a combinação é feita em memória com LINQ. Nunca uma consulta por jogo.
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
            // "into" agrupa os vínculos de cada jogo (um jogo tem vários gêneros)
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
