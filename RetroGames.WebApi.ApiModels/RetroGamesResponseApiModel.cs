namespace RetroGames.WebApi.ApiModels;

// Envelope de toda resposta da API. A versão sem Data é usada nos erros.
public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public T? Data { get; set; }

    public static RetroGamesResponseApiModel<T> Sucesso(T data) => new()
    {
        ResponseCode = ResponseCodes.SUCESSO,
        Data = data,
    };
}
