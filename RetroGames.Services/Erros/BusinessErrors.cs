using System.Net;

namespace RetroGames.Services.Erros;

// Catálogo único dos erros de negócio. Erro novo recebe o próximo código livre.
public static class BusinessErrors
{
    public static readonly ErroNegocio PLATAFORMA_NAO_ENCONTRADA =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio PLATAFORMA_NOME_DUPLICADO =
        new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio PLATAFORMA_POSSUI_JOGOS =
        new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio JOGO_NAO_ENCONTRADO =
        new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio CONDICAO_NAO_ENCONTRADA =
        new(104, "A condição informada não existe.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio GENERO_NAO_ENCONTRADO =
        new(105, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}
