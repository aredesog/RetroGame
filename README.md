# RetroGames

Loja web de jogos retrô usados, desenvolvida ao longo do curso de desenvolvimento web. O projeto é construído em sprints semanais, e este repositório registra a evolução a cada aula.

O escopo atual é o **Módulo de Catálogo e Administração**.

## O que vai ser feito

- [ ] Estrutura da solução (.NET) e do front-end (React)
- [ ] Banco de dados MySQL: tabelas de plataforma, gênero, jogo, condição, perfil e usuário
- [ ] Autenticação e autorização do perfil Administrador (sessão por cookie e proteção CSRF)
- [ ] Manutenção de plataformas (incluir, consultar, alterar e excluir)
- [ ] Consulta de gêneros e de condições de jogo
- [ ] Manutenção de jogos, com associação a gêneros e exclusão lógica
- [ ] Catálogo público com busca, filtros por plataforma e gênero e paginação
- [ ] Página de detalhe do jogo
- [ ] Testes unitários dos controllers, services e validators

## Stack

| Camada | Tecnologia |
| --- | --- |
| API | .NET 10, ASP.NET Core (controllers) |
| Acesso a dados | ADO.NET com MySqlConnector |
| Banco de dados | MySQL 8 |
| Validação | FluentValidation |
| Testes | xUnit + Moq |
| Front-end | React + Vite, Bootstrap, React Router, Axios |

## Arquitetura

O back-end segue arquitetura em camadas:

```
Controller → Service → Repository → MySQL
```

- **Controller:** recebe a requisição e devolve a resposta.
- **Service:** aplica as regras de negócio e controla as transações.
- **Repository:** executa o SQL parametrizado.

Todas as respostas da API usam o mesmo formato:

```json
{ "responseCode": 0, "data": { }, "errorMessage": null }
```

## Como executar

Pré-requisitos: .NET SDK 10, Node.js 22 LTS e MySQL 8.

1. Criar o banco com os scripts da pasta `database/`.
2. Configurar a connection string e o administrador inicial com `dotnet user-secrets` no projeto `RetroGames.WebApi`.
3. Rodar a API: `dotnet run` em `RetroGames.WebApi` (porta 5080, Swagger em `/swagger`).
4. Rodar o front-end: `npm install` e `npm run dev` em `retrogames.web` (porta 5173).
5. Rodar os testes: `dotnet test` na raiz.

> As instruções de execução serão confirmadas conforme as etapas forem implementadas.