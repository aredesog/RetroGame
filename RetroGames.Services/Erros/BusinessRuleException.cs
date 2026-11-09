namespace RetroGames.Services.Erros;

// Lançada quando uma regra é violada. A API lê o Erro para montar a resposta.
public class BusinessRuleException : Exception
{
    public ErroNegocio Erro { get; }

    public BusinessRuleException(ErroNegocio erro)
        : base(erro.Mensagem)
    {
        Erro = erro;
    }
}
