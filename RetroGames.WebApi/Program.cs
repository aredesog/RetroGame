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

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Primeira etapa do pipeline: assim pega exceção de qualquer etapa depois dela.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // descrição da API em /openapi/v1.json
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API");
        opcoes.RoutePrefix = "swagger";
    });
}

app.MapControllers();

app.Run();
