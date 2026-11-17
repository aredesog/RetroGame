using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

// Roda antes de toda action. Para cada argumento que tem um validator registrado,
// valida o formato; se houver erro, responde 400 e a action nem é chamada.
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider serviceProvider;

    public FluentValidationFilter(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Proteção extra: parâmetro que deveria vir do corpo chegou nulo.
        var corpoAusente = context.ActionDescriptor.Parameters.Any(p =>
            p.BindingInfo?.BindingSource == BindingSource.Body &&
            (!context.ActionArguments.TryGetValue(p.Name, out var valor) || valor is null));

        if (corpoAusente)
        {
            context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.MODEL_NULL.Code,
                ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
            });
            return;
        }

        var errosPorCampo = new Dictionary<string, string[]>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null)
            {
                continue;
            }

            // Procura um IValidator<TipoDoArgumento>; o int do id da rota, por exemplo, não tem.
            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (serviceProvider.GetService(tipoValidator) is not IValidator validator)
            {
                continue;
            }

            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento));

            // Agrupa as mensagens por campo, com o nome em camelCase, igual ao JSON.
            foreach (var grupo in resultado.Errors.GroupBy(e => e.PropertyName))
            {
                errosPorCampo[JsonNamingPolicy.CamelCase.ConvertName(grupo.Key)] =
                    grupo.Select(e => e.ErrorMessage).ToArray();
            }
        }

        if (errosPorCampo.Count > 0)
        {
            context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                Data = errosPorCampo,
            });
            return;
        }

        await next();
    }
}
