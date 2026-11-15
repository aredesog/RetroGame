using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static JogoFiltroServiceModel ParaFiltro(this ListarJogoRequestApiModel request) => new()
    {
        Busca = request.Busca,
        IdPlataforma = request.IdPlataforma,
        IdGenero = request.IdGenero,
        Pagina = request.Pagina,
        TamanhoPagina = request.TamanhoPagina,
    };

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaResponse(
        this PaginaServiceModel<JogoDetalheServiceModel> pagina) => new()
    {
        Itens = pagina.Itens.Select(d => d.ParaListarResponse()).ToList(),
        Pagina = pagina.Pagina,
        TamanhoPagina = pagina.TamanhoPagina,
        TotalItens = pagina.Total,
        // Divisão arredondada para cima: 23 jogos em páginas de 5 = 5 páginas.
        TotalPaginas = pagina.TamanhoPagina > 0
            ? (int)Math.Ceiling(pagina.Total / (double)pagina.TamanhoPagina)
            : 0,
    };

    public static ListarJogoResponseApiModel ParaListarResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        NomePlataforma = detalhe.NomePlataforma,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(g => g.Nome).ToList(),
    };

    public static ListarPorIdJogoResponseApiModel ParaListarPorIdResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        Descricao = detalhe.Jogo.Descricao,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        IdPlataforma = detalhe.Jogo.IdPlataforma,
        NomePlataforma = detalhe.NomePlataforma,
        IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(g => g.ParaResponse()).ToList(),
    };
}
