using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

// Lê o .env (procurando nas pastas acima até achar) e coloca cada linha como variável de ambiente.
// Precisa vir antes do CreateBuilder, que é quando a configuração é lida.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// No .env a chave é ConnectionStrings__Default; o "__" vira ":" na configuração.
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string não configurada. Crie o arquivo .env na raiz do repositório " +
        "(copie de .env.example) com a linha ConnectionStrings__Default=\"...\".");
}

// Administrador inicial (RS05): também vem do .env.
var emailAdmin = builder.Configuration["AdminInicial:Email"];
var senhaAdmin = builder.Configuration["AdminInicial:Senha"];
if (string.IsNullOrWhiteSpace(emailAdmin) || string.IsNullOrWhiteSpace(senhaAdmin))
{
    throw new InvalidOperationException(
        "Administrador inicial não configurado. Acrescente ao .env as linhas " +
        "AdminInicial__Email=\"...\" e AdminInicial__Senha=\"...\" (veja .env.example).");
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

builder.Services.AddAutenticacaoPorCookie();
builder.Services.AddAutorizacao();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Registra todos os validators deste projeto (cada classe que herda de AbstractValidator).
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services
    .AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        // Dado que nem chega a virar objeto (JSON quebrado, "pagina=abc"):
        // o ASP.NET Core recusa antes do filtro. Responde no envelope, sem lista de campos.
        opcoes.InvalidModelStateResponseFactory = _ => new BadRequestObjectResult(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
        });
    });
builder.Services.AddOpenApi();

var app = builder.Build();

// Cria o administrador se ainda não houver nenhum. Os Services são "scoped" (um por requisição),
// então fora de uma requisição é preciso abrir um escopo à mão.
using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(emailAdmin, senhaAdmin);
}

// Primeira etapa do pipeline: assim pega exceção de qualquer etapa depois dela.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();   // descrição da API em /openapi/v1.json; sem login, o Swagger não carrega
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API");
        opcoes.RoutePrefix = "swagger";
    });
}

// Nessa ordem: primeiro descobre quem é (cookie), depois decide o que pode.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
