# Mapa da estrutura do repositório

Monólito **.NET 8** em camadas, com separação clara de responsabilidades. Arquitetura alinhada ao padrão **Seven.Support.V3** e ao **Guia prático: Criação de CRUD do zero** (15 passos + MapperProfile).

## Visão geral das camadas

| Camada | Pasta | Papel |
|--------|-------|-------|
| **Domain** | `src/Domain/` | Entidades, interfaces e mapeadores; C# puro, sem dependências externas além das bibliotecas standard |
| **Application** | `src/Application/` | App services (`FinanceControl.Services`) e pontos de entrada: **API REST** (`FinanceControl.Web.Api`) e **MVC** (`FinanceControl.Web.App`) |
| **Infrastructure** | `src/Infrastructure/` | Acesso a dados, repositórios, Entity Framework Core com MySQL |
| **Crosscutting** | `src/Crosscutting/` | Contratos partilhados (DTOs, interfaces de entidade, filtros) e cultura/localização |
| **Presentation** | `src/Presentation/` | Client services (`HttpClient` → API) e UI MVC |
| **Tests** | `src/Tests/` | Testes de integração xUnit com `FinanceApiFactory` |

A **Web** consome a **API** via `CliService` (não acede directamente ao domínio nem à base de dados).

## Fluxo de pedido (runtime)

```
Controller (MVC) → CliService → POST/GET /api/[Recurso]
  → API Controller → AppService → Repository + DomService → DbContext → DTO
```

- Validação de modelo: `ModelState.IsValid` nos controllers da API.
- Regras de negócio e validações customizadas: `{Entidade}DomService`.

## Solução (`FinanceControl.sln`)

Abra no Visual Studio com **Solution View** (não *Folder View*). Use `FinanceControl.sln` (formato clássico).

| Pasta de solução | Projeto | Pasta no disco |
|------------------|---------|----------------|
| **Crosscutting** | `FinanceControl.Contracts` | `src/Crosscutting/FinanceControl.Contracts/` |
| | `FinanceControl.Culture` | `src/Crosscutting/FinanceControl.Culture/` |
| **Domain** | `FinanceControl.Domain` | `src/Domain/FinanceControl.Domain/` |
| | `FinanceControl.Domain.Services` | `src/Domain/FinanceControl.Domain.Services/` |
| **Infrastructure** | `FinanceControl.Data` | `src/Infrastructure/FinanceControl.Data/` |
| **Application** | `FinanceControl.Services` | `src/Application/FinanceControl.Services/` |
| | `FinanceControl.Web.Api` | `src/Application/FinanceControl.Web.Api/` |
| **Presentation** | `FinanceControl.Client.Services` | `src/Presentation/FinanceControl.Client.Services/` |
| | `FinanceControl.Web` | `src/Presentation/FinanceControl.Web.App/` |
| **Tests** | `FinanceControl.Tests` | `src/Tests/FinanceControl.Tests/` |

## Fluxo CRUD (guia → pastas)

| Passo | Artefacto | Onde criar |
|-------|-----------|------------|
| 1 | `I{Entidade}` | `Contracts/Interfaces/Entities/{Entidade}/` |
| 2 | `{Entidade}` | `Domain/Entities/{Entidade}/` |
| 3 | `{Entidade}Dto` | `Contracts/Dtos/{Entidade}/` |
| 4 | `{Entidade}Configuration` | `Data/EntityConfiguration/{Entidade}/` |
| 5 | `I{Entidade}Repository` | `Domain/Interfaces/Repositories/{Entidade}/` |
| 6 | `{Entidade}Repository` | `Data/Repositories/{Entidade}/` |
| 7 | `I{Entidade}AppService` | `Domain/Interfaces/AppServices/{Entidade}/` |
| 8 | `{Entidade}AppService` | `Services/{Entidade}/` |
| 9 | `{Entidade}Controller` (API) | `Web.Api/Controllers/{Entidade}/` |
| 10 | `I{Entidade}CliService` | `Client.Services/Interfaces/{Entidade}/` |
| 11 | `{Entidade}CliService` | `Client.Services/Integrated/{Entidade}/` |
| 12–15 | Controller / View / JS / ViewModel (UI) | `Web.App` (MVC — ver abaixo) |
| — | Mapper (estático) | `Domain/MapperProfiles/{Entidade}/` |
| — | Registo DI | `Web.Api/Program.cs` (e `Web.App` quando aplicável) |

**Entidades actuais:** `Accounts`, `Categories`, `Transactions`, `TransactionTypes`, `Users`.

> No domínio deste projeto, **meios de pagamento** correspondem à entidade `TransactionTypes` (rotas `/tipos-transacao`), não a um recurso separado `PaymentMethod`.

### Relacionamentos entre entidades

- **Users:** entidade pai de contas, categorias e transações.
- **Accounts:** possuem transações.
- **Categories:** escopadas por utilizador (multi-tenant por user).
- **Transactions:** ligam `User` → `Account` → `Category` + `TransactionType`.

## Árvore por camada

### Crosscutting — `FinanceControl.Contracts`
```
Constants/
Dtos/{Entidade}/
Dtos/Auth/
Dtos/Common/
Enumerators/
Filters/          ← DataFilterDto, DataResultDto
Interfaces/Entities/{Entidade}/
Validations/
FinanceControlContractsModule.cs
```

### Crosscutting — `FinanceControl.Culture`
```
Environments/
MailTemplates/
Validations/
FinanceControlCultureModule.cs
```

### Domain — `FinanceControl.Domain`
```
Entities/{Entidade}/
Enums/
Interfaces/AppServices|Repositories|DomService/{Entidade}/
MapperProfiles/{Entidade}/     ← classes estáticas (ex.: AccountMapper.ToDto)
FinanceControlDomainModule.cs
```

### Domain — `FinanceControl.Domain.Services`
```
{Entidade}/{Entidade}DomService.cs
FinanceControlDomainServiceModule.cs
```

### Infrastructure — `FinanceControl.Data`
```
Contexts/                      ← FinanceDbContext
EntityConfiguration/{Entidade}/
Migrations/
Repositories/{Entidade}/
Seeding/
FinanceControlDataModule.cs
```

### Application — `FinanceControl.Services`
```
{Entidade}/{Entidade}AppService.cs
FinanceControlAppServicesModule.cs
```

### Application — `FinanceControl.Web.Api`
```
Authentication/
Authorization/
Configuration/
Controllers/{Entidade}/
Extensions/
AppConsts.cs
Program.cs                     ← DI (scoped), DbContext, migrations
FinanceControlWebApiModule.cs
```

### Presentation — `FinanceControl.Client.Services`
```
Constants.cs
DependencyInjection/       ← registo HttpClient
Integrated/{Entidade}/     ← CliService
Interfaces/{Entidade}/     ← ICliService
Options/                   ← ApiClientOptions
FinanceControlClientServicesModule.cs
```

### Presentation — `FinanceControl.Web.App` (MVC)
```
Controllers/
  Auth/                    ← Login
  Home/
  Register/
  {Entidade}/              ← CRUD (Categories, Transactions, …)
  Dashboards/
  Reports/
  Profile/
  Account/
  Error/
Infrastructure/DependencyInjection/
Models/ViewModels/{Entidade}/
Options/
Views/
  Shared/                  ← _Layout, _Nav, partials
  {Entidade}/Index.cshtml
wwwroot/css/
wwwroot/js/{entidade}/
wwwroot/lib/
FinanceControlWebAppModule.cs
Program.cs
```

**Rotas principais da UI**

| URL | Controller |
|-----|------------|
| `/` | `Login` |
| `/home` | `Home` |
| `/register` | `Register` |
| `/transacoes` | `Transactions` |
| `/categorias` | `Categories` |
| `/tipos-transacao` | `TransactionTypes` (meios de pagamento) |
| `/dashboards/geral` | `Dashboards` |
| `/dashboards/por-categoria` | `Dashboards` |
| `/dashboards/por-transacoes` | `Dashboards` |
| `/relatorios` | `Reports` → redirect dashboards |
| `/perfil` | `Profile` |
| `/conta/configuracao`, `/conta/privacidade` | `Account` |
| `/transactions` | `Transactions` → redirect `/transacoes` |
| `/health` | `Health` |

> **Não usar `Pages/`** — a UI é só **ASP.NET Core MVC** (`Controllers` + `Views`). Razor Pages não está em uso.

## Padrões técnicos

### Injeção de dependência

- Serviços registados como **scoped** (vida útil do pedido HTTP) em `Program.cs` da API.
- Padrão por entidade: `IXxxRepository` → `XxxRepository`, `IXxxDomService` → `XxxDomService`, `IXxxAppService` → `XxxAppService`.

Exemplo (`Account`):

```csharp
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountDomService, AccountDomService>();
builder.Services.AddScoped<IAccountAppService, AccountAppService>();
```

Interfaces em **Domain**; implementações de repositório em **Infrastructure**; app services em **Application/Services**; dom services em **Domain.Services**.

### Mapeamento de dados

- Classes estáticas em `Domain/MapperProfiles/{Entidade}/` convertem entidade ↔ DTO.
- Exemplo: `AccountMapper.ToDto(entity)` → `AccountDto`.
- Mappers no Domain evitam dependências circulares.

### Filtros e paginação

- Consultas paginadas: `DataFilterDto` (`page`, `pageSize`, dicionário de filtros).
- Resposta: `DataResultDto<T>` com `ICollection<T>`, página e total.
- Exemplo: `GET /api/Account?page=1&pageSize=10&userId=5`

### Interfaces por camada (exemplo `Account`)

| Camada | Interface | Implementação |
|--------|-----------|-----------------|
| Domain | `IAccountRepository` | `AccountRepository` (Infrastructure) |
| Domain | `IAccountDomService` | `AccountDomService` (Domain.Services) |
| Domain | `IAccountAppService` | `AccountAppService` (Services) |
| Presentation | `IAccountCliService` | `AccountCliService` (Client.Services) |

### Convenções de código

1. **Nullability:** `#nullable enable` em todos os projetos; `?` para propriedades opcionais.
2. **Async:** operações I/O assíncronas com parâmetro `CancellationToken`.
3. **DI:** sem dependências estáticas; tudo via injeção scoped.
4. **Validação:** `ModelState.IsValid` nos controllers; lógica customizada no `DomService`.
5. **Nomes:** `IXxxService`, `XxxService`, `XxxDto`, `XxxMapper`.

## Módulos (`*Module.cs`)

Cada projeto tem um ficheiro `*Module.cs` como ponto de registo DI (padrão Seven). A implementação dos registos fica em `Program.cs` (API/Web) ou `DependencyInjection/` (Client.Services e Web.App).

## Comandos úteis

```bash
dotnet build FinanceControl.sln
dotnet test
# ou
dotnet test src/Tests/FinanceControl.Tests/FinanceControl.Tests.csproj
```

### Testes

- **Tipo:** xUnit com `FinanceApiFactory` (Web Application Factory).
- **Local:** `src/Tests/FinanceControl.Tests/Integration/`
- **Exemplos:** `AccountCrudTests`, `CategoryCrudTests`, `TransactionCrudTests` — fluxo HTTP completo, sem mocks.
- **Execução:** `dotnet test` na raiz da solução.

## Banco de dados (MySQL)

- **DbContext:** `FinanceDbContext` em `Infrastructure/Contexts/`
- **Provider:** MySQL via `Pomelo.EntityFrameworkCore.MySql`
- **Migrations:** `Infrastructure/FinanceControl.Data/Migrations/` — ex.: `InitialCreate`, `AddCategoryIcon`, `AddPaymentMethodIcon`
- **Seed:** aplicado automaticamente ao iniciar a API (excepto ambiente `Testing`)

Ver detalhes em **[DATABASE.md](DATABASE.md)**.

- Ao iniciar a **API**, migrations + seed de demonstração rodam automaticamente.
- Manualmente:

```powershell
cd src/Infrastructure/FinanceControl.Data
dotnet ef migrations add NomeDaMigration --startup-project ../../Application/FinanceControl.Web.Api
dotnet ef database update --startup-project ../../Application/FinanceControl.Web.Api
```

Connection string: `src/Application/FinanceControl.Web.Api/appsettings.Development.json`.

## Arranque e debug local (F5)

| Projecto | Pasta | URL (Development) |
|----------|-------|-------------------|
| **Site (UI)** | `FinanceControl.Web` → `FinanceControl.Web.App` | `https://localhost:7143` |
| **API** | `FinanceControl.Web.Api` | `https://localhost:7189` (Swagger: `/swagger`) |

A API configura CORS para a origem da Web (`https://localhost:7143`). Ambos requerem MySQL em `appsettings.Development.json`.

## Ficheiros críticos

- `src/Application/FinanceControl.Web.Api/Program.cs` — DI, DbContext, arranque
- `src/Infrastructure/FinanceControl.Data/Contexts/FinanceDbContext.cs` — mapeamentos EF
- `src/Crosscutting/FinanceControl.Contracts/` — DTOs e contratos
- `README.md` — requisitos funcionais (português)

## Notas

- **.NET 8** com `Nullable` activado em todos os projetos.
- **JWT** em análise (ainda não implementado).
- CRUDs de Categorias, meios de pagamento (`TransactionTypes`) e Transações usam modal + `CliService` + API REST.
- `IFinanceControlApiClient` permanece para login, registo e dashboard; entidades CRUD usam `I{Entidade}CliService`.
- Para agentes de IA (Copilot, etc.), ver também [`.github/copilot-instructions.md`](.github/copilot-instructions.md).
