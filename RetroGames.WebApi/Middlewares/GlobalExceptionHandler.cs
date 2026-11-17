using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services.Erros;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

// Recebe toda exceção que nenhum código tratou e devolve o envelope de erro.
// Por isso os controllers não precisam de try/catch.
public class GlobalExceptionHandler : IExceptionHandler
{
    private const string MensagemGenerica = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException erroDeNegocio)
        {
            // Erro previsto: o catálogo já diz o status, o código e a mensagem.
            status = (int)erroDeNegocio.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = erroDeNegocio.Erro.Code,
                ErrorMessage = erroDeNegocio.Message,
            };
        }
        else
        {
            // Erro imprevisto: detalhes (SQL, tabela, caminho) só no log, nunca na resposta.
            logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);

            status = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = MensagemGenerica,
            };
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true; // true = exceção tratada, o ASP.NET Core não faz mais nada com ela
    }
}
