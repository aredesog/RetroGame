using System.Net;

namespace RetroGames.Services.Erros;

/// <summary>
/// Catálogo dos erros de negócio. Cada erro é definido uma única vez, aqui.
/// Códigos a partir de 100 (Aula 3); um erro novo recebe o próximo número livre.
/// </summary>
public static class BusinessErrors
{
    /// <summary>Plataforma pedida ou do jogo não existe (RN05).</summary>
    public static readonly ErroNegocio PLATAFORMA_NAO_ENCONTRADA =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    /// <summary>Nome de plataforma já usado (RN01).</summary>
    public static readonly ErroNegocio PLATAFORMA_NOME_DUPLICADO =
        new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    /// <summary>Plataforma com jogos não pode ser excluída (RN02).</summary>
    public static readonly ErroNegocio PLATAFORMA_POSSUI_JOGOS =
        new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    /// <summary>O id do jogo não existe.</summary>
    public static readonly ErroNegocio JOGO_NAO_ENCONTRADO =
        new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    /// <summary>Condição do jogo não existe (RN05).</summary>
    public static readonly ErroNegocio CONDICAO_NAO_ENCONTRADA =
        new(104, "A condição informada não existe.", HttpStatusCode.NotFound);

    /// <summary>Gênero do jogo não existe (RN05).</summary>
    public static readonly ErroNegocio GENERO_NAO_ENCONTRADO =
        new(105, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}
