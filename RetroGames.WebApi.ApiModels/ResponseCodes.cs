namespace RetroGames.WebApi.ApiModels;

// Códigos que nascem na própria API (0 a 99). Os erros de negócio (100 em diante)
// ficam no BusinessErrors do Services e não se repetem aqui.
public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;

    public static readonly (int Code, string DefaultMessage) MODEL_NULL =
        (2, "O modelo veio nulo.");

    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO =
        (3, "Há um erro com o modelo enviado.");
}
