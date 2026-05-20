# Mapa da estrutura do repositório

Arquitetura alinhada ao padrão **Seven.Support.V3** e ao **Guia prático: Criação de CRUD do zero** (15 passos + MapperProfile).

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
| — | MapperProfile | `Domain/MapperProfiles/{Entidade}/` |

**Entidades actuais:** `Accounts`, `Categories`, `Transactions`, `TransactionTypes`, `Users`.

## Árvore por camada

### Crosscutting — `FinanceControl.Contracts`
```
Constants/
Dtos/{Entidade}/
Enumerators/
Filters/          ← DataFilterDto
Interfaces/Entities/{Entidade}/
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
Interfaces/AppServices|Repositories|DomService/{Entidade}/
MapperProfiles/{Entidade}/
Validators/{Entidade}/
Models/{Entidade}/
Views/{Entidade}/
FinanceControlDomainModule.cs
```

### Domain — `FinanceControl.Domain.Services`
```
{Entidade}/{Entidade}DomService.cs
FinanceControlDomainServiceModule.cs
```

### Infrastructure — `FinanceControl.Data`
```
Contexts/
DatabaseObjects/
EntityConfiguration/{Entidade}/
MapperProfiles/{Entidade}/
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
Controllers/{Entidade}/    ← rotas da UI
Models/ViewModels/{Entidade}/
Views/
  Shared/                  ← _Layout, _Nav, partials
  {Entidade}/Index.cshtml
wwwroot/js/{entidade}/
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
| `/tipos-transacao` | `TransactionTypes` |
| `/dashboards/geral` | `Dashboards` |
| `/relatorios` | `Reports` → redirect dashboards |
| `/transactions` | `Transactions` → redirect `/transacoes` |

> **Não usar `Pages/`** — a UI é só MVC (`Controllers` + `Views`).

## Módulos (`*Module.cs`)

Cada projeto tem um ficheiro `*Module.cs` como ponto de registo DI (padrão Seven). A implementação dos registos fica em `Program.cs` (API/Web) ou `DependencyInjection/` (Client.Services).

## Comandos úteis

```bash
dotnet build FinanceControl.sln
dotnet test src/Tests/FinanceControl.Tests/FinanceControl.Tests.csproj
```

## Banco de dados (MySQL)

Uma única migration `InitialCreate` cria todas as tabelas. Ver **[DATABASE.md](DATABASE.md)**.

- Ao iniciar a **API**, migrations + seed de demonstração rodam automaticamente.
- Ou manualmente: `dotnet ef database update` em `FinanceControl.Data` (startup: Web.Api).

## Arranque (F5)

- **Site (UI):** `FinanceControl.Web` → pasta `FinanceControl.Web.App`
- **API:** `FinanceControl.Web.Api`

## Notas

- CRUDs de Categorias, Meios de pagamento e Transações usam modal + `CliService` + API REST.
- `IFinanceControlApiClient` permanece para login/registo e dashboard; entidades CRUD usam `I{Entidade}CliService`.
