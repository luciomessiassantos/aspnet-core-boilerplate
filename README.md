# ⚙️ ASP.NET Core Boilerplate

Um boilerplate/starter kit para APIs em **.NET 10**, reunindo configurações e componentes comuns a praticamente qualquer projeto real: autenticação JWT com ASP.NET Identity, tratamento global de exceções, *rate limiting*, cache híbrido e utilitários de paginação/consulta — tudo pronto para servir de base a novos projetos.

---

## ✨ Funcionalidades incluídas

- **Autenticação JWT** integrada ao **ASP.NET Identity** (`User : IdentityUser<Guid>`), com suporte a leitura do token via cookie (`access_token`)
- **Autorização baseada em política** (ex: `RequireAdminRole`)
- **Rate Limiting** com quatro estratégias já configuradas: *Fixed Window*, *Sliding Window*, *Token Bucket* e *Concurrency Limiter*
- **Tratamento global de exceções** via `IExceptionHandler` (`GlobalExceptionHandler` e `ValidationExceptionHandler`), retornando respostas no padrão `ProblemDetails`
- **Cache híbrido** (memória local + Redis) via `HybridCache`
- **Paginação genérica** (`PaginatedResult<T>`) e extensão `WhereIf` para montar queries condicionais com `IQueryable`
- **CORS** configurável por lista de origens
- **Health Checks** prontos para uso
- Suporte simultâneo a **PostgreSQL** e **SQL Server** (basta habilitar o provedor desejado)
- Documentação da API via **Swagger / OpenAPI**

---

## 🛠️ Tecnologias

| Categoria | Tecnologia |
|---|---|
| Framework | .NET 10 / ASP.NET Core Web API |
| Autenticação | ASP.NET Identity + JWT Bearer |
| Mediador (CQRS) | MediatR |
| Persistência | Entity Framework Core (Npgsql / SQL Server) |
| Cache | Redis + `HybridCache` |
| Agendamento | Quartz.NET (dependência já incluída) |
| E-mail | MailKit |
| Documentação da API | Swashbuckle (Swagger) / OpenAPI |

---

## 🏗️ Estrutura do projeto

```
src/
├── Domain/                          → Entidades de domínio (ex: User)
├── Infrastructure/                  → DbContexts (Application / Identity)
├── Presentation/                    → Controller base da aplicação
└── Shared/
    ├── Config/                      → Configurações (ex: JwtSettings)
    ├── Exceptions/
    │   ├── Handlers/                → GlobalExceptionHandler, ValidationExceptionHandler
    │   └── Implementations/         → ValidationException
    └── Utils/                       → PaginatedResult, QueryableExtensions, ICrudService, IMapper, ITokenService
```

### Destaques de implementação

- **`User`**: estende `IdentityUser<Guid>`, adicionando `CpfCnpj` e suporte a *refresh token*.
- **`PaginatedResult<T>`**: encapsula dados paginados com `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`, `HasPreviousPage` e `HasNextPage`.
- **`QueryableExtensions.WhereIf`**: aplica um filtro condicionalmente em uma única linha, evitando `if`s espalhados na montagem de queries.
- **`QueryableExtensions.ToPaginatedResultAsync`**: converte qualquer `IQueryable` em um `PaginatedResult` já projetado (`Select`) e paginado.

---

## 🚀 Como usar este boilerplate

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **PostgreSQL** e/ou **SQL Server**
- **Redis** em execução

### Configuração

1. Clone o repositório (ou use como template para um novo projeto):
   ```bash
   git clone https://github.com/luciomessiassantos/aspnet-core-boilerplate.git
   cd aspnet-core-boilerplate
   ```

2. Configure as strings de conexão e o segredo JWT no `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Postgres": "Host=localhost;Port=5432;Database=app;Username=postgres;Password=postgres",
       "SQLServer": "Server=localhost;Database=app;Trusted_Connection=True;",
       "Redis": "localhost:6379"
     },
     "Jwt": {
       "Key": "sua-chave-secreta-com-pelo-menos-32-caracteres",
       "Issuer": "aspnet-core-boilerplate",
       "Audience": "aspnet-core-boilerplate"
     }
   }
   ```

3. No `Program.cs`, habilite o provedor de banco desejado para o `IdentityAppDbContext` (Postgres ou SQL Server — ambas as opções já estão presentes, comentadas).

4. Execute a aplicação:
   ```bash
   dotnet run
   ```

5. Acesse a documentação da API em `/swagger`.

---

## 🔐 Autenticação

O token JWT pode ser enviado tanto no header `Authorization: Bearer <token>` quanto automaticamente via cookie `access_token`, conforme configurado em `JwtBearerEvents.OnMessageReceived`.

---

## 🚦 Rate Limiting

Quatro políticas já vêm configuradas e podem ser aplicadas por endpoint com `[EnableRateLimiting("nome-da-politica")]`:

| Política | Estratégia |
|---|---|
| `fixed` | Fixed Window |
| `sliding` | Sliding Window |
| `token` | Token Bucket |
| `concurrency` | Concurrency Limiter |

---

## 📄 Licença

Este projeto está disponível para fins de estudo e como base (template) para novos projetos ASP.NET Core.
