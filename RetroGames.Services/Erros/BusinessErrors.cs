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
        new(104, "Condição de jogo não encontrada.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio GENERO_NAO_ENCONTRADO =
        new(105, "Gênero não encontrado.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio EMAIL_JA_CADASTRADO =
        new(106, "Já existe uma conta com este e-mail.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio GENERO_NOME_DUPLICADO =
        new(107, "Já existe um gênero com este nome.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio GENERO_POSSUI_JOGOS =
        new(108, "Não é possível excluir: existem jogos com este gênero.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio PEDIDO_NAO_ENCONTRADO =
        new(109, "Pedido não encontrado.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio JOGO_INDISPONIVEL =
        new(110, "Um ou mais jogos não estão disponíveis para venda.", HttpStatusCode.BadRequest);

    public static readonly ErroNegocio ESTOQUE_INSUFICIENTE =
        new(111, "Estoque insuficiente.", HttpStatusCode.Conflict);

    public static readonly ErroNegocio SITUACAO_NAO_ENCONTRADA =
        new(112, "Situação de pedido não encontrada.", HttpStatusCode.NotFound);

    public static readonly ErroNegocio PEDIDO_DE_OUTRO_USUARIO =
        new(113, "Você não tem acesso a este pedido.", HttpStatusCode.Forbidden);
}
