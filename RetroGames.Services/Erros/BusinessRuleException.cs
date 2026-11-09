namespace RetroGames.Services.Erros;

/// <summary>
/// Lançada pelo Service quando uma regra de negócio é violada.
/// Guarda o erro do catálogo; na aula 4, a API responde com o status e o código dele.
/// </summary>
public class BusinessRuleException : Exception
{
    public ErroNegocio Erro { get; }

    public BusinessRuleException(ErroNegocio erro)
        : base(erro.Mensagem)
    {
        Erro = erro;
    }
}
