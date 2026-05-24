# Copilot Instructions for FinanceControl Project

## Architecture Overview

This is a **layered .NET 8 monolith** with clear separation of concerns:

- **Domain** (`src/Domain/`): Core business logic and entities; pure C# with no external dependencies except standard libraries
- **Application** (`src/Application/`): Service layer (app services, domain services) and two client entry points:
  - **API** (`FinanceControl.Web.Api`): REST API consuming domain/app services
  - **Web** (`FinanceControl.Web.App`): ASP.NET Core MVC/Razor Pages application consuming the API via `HttpClient`
- **Infrastructure** (`src/Infrastructure/`): Data access, repositories, Entity Framework Core with MySQL
- **Crosscutting** (`src/Crosscutting/`): Shared contracts (DTOs, interfaces, filters)
- **Presentation** (`src/Presentation/`): Client services that wrap HTTP calls to the API
- **Tests** (`src/Tests/`): xUnit integration tests with `FinanceApiFactory`

## Key Patterns

### Request Flow (Web App → API)
1. **Controller** (MVC) receives request → invokes **CliService** (HTTP client)
2. **CliService** posts JSON to `/api/[Resource]` endpoint
3. **API Controller** validates and delegates to **AppService**
4. **AppService** orchestrates **Repository** + **DomService** (domain logic)
5. **Repository** queries/persists via **DbContext** → **DTO** returned to client

### Dependency Injection
- All services registered in `Program.cs` as **scoped** (request-lifetime)
- Pattern: `IXxxRepository` → `XxxRepository`, `IXxxAppService` → `XxxAppService`, `IXxxDomService` → `XxxDomService`
- Example (Accounts):
  ```csharp
  builder.Services.AddScoped<IAccountRepository, AccountRepository>();
  builder.Services.AddScoped<IAccountDomService, AccountDomService>();
  builder.Services.AddScoped<IAccountAppService, AccountAppService>();
  ```

### Data Mapping
- **Mappers** (static methods in `MapperProfiles/`) convert entities ↔ DTOs
- Example: `AccountMapper.ToDto(entity)` converts `Account` → `AccountDto`
- Mappers live in Domain layer to avoid circular dependencies

### Entity Relationships
- **Users**: Parent entity for Accounts, Categories, Transactions
- **Accounts**: Own Transactions
- **Categories**: Scoped to User (multi-tenant per user)
- **Transactions**: Link User → Account → Category + TransactionType

## Database & Migrations

- **DbContext**: `FinanceDbContext` in `Infrastructure/Contexts/`
- **Connection**: MySQL via `Pomelo.EntityFrameworkCore.MySql`
- **Migrations**: Located in `Infrastructure/Migrations/`; generated via `dotnet ef migrations add`
- **Seeding**: Auto-applied on startup unless `Testing` environment (see `Program.cs`)

```csharp
builder.Services.AddDbContext<FinanceDbContext>(options =>
	options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));
```

## Testing

- **Type**: xUnit with `FinanceApiFactory` (Web Host factory)
- **Location**: `src/Tests/FinanceControl.Tests/`
- **Run**: `dotnet test` from solution root
- **Example**: `AccountCrudTests` — tests full HTTP flow without mocking

## Key Interfaces & Namespaces

| Layer | Interface | Implementation |
|-------|-----------|-----------------|
| Contracts | `IAccountAppService` | `AccountAppService` |
| Domain | `IAccountRepository` | `AccountRepository` (Infrastructure) |
| Domain | `IAccountDomService` | `AccountDomService` |
| Presentation | `IAccountCliService` | `AccountCliService` (HTTP wrapper) |

## DTO Filters

- Use `DataFilterDto` for paginated queries (page, pageSize, filters dict)
- Result: `DataResultDto<T>` wraps `ICollection<T>`, page, total
- Example: `/api/Account?page=1&pageSize=10&userId=5`

## Coding Conventions

1. **Nullability**: `#nullable enable` in all projects; use `?` for optional properties
2. **Async/await**: All I/O operations are async (`async`/`await`); no `CancellationToken` in method signatures
3. **Scoped lifetimes**: No static dependencies; DI everywhere
4. **DTO validation**: `ModelState.IsValid` in controllers; custom logic in `DomService`
5. **Naming**: `IXxxService` (interface), `XxxService` (class), `XxxDto` (contract), `XxxMapper` (mapper static class)

## ⚠️ Workflow Requirements for Code Changes

**MANDATORY - NEVER SKIP THIS**: Before making ANY changes to the codebase:

1. **Create a Detailed Step-by-Step Plan** — Present EXACTLY what you will do:
   - List EVERY file that will be modified or created
   - Show file paths (full relative paths)
   - For each file: specify old content → new content
   - Explain WHY each change is needed
   - How it aligns with architecture
   - Potential impacts or dependencies
   - Any build/test implications

   **Format the plan clearly** so the user can easily review and understand EVERY change before it happens.

2. **Wait for Explicit User Authorization** — Do NOT execute ANY code changes until the user explicitly says "approve", "ok", "go ahead", or similar approval

3. **Ask for Approval Every Time** — Even if you presented a similar plan before, always ask again and wait for approval

4. **Execute Only After Approval** — Once explicitly approved, proceed with the implementation step-by-step

**This ensures intentional, well-reviewed changes and prevents accidental modifications.**

**NEVER assume silence is approval. ALWAYS wait for explicit confirmation.**

## Common Tasks

### Add a New CRUD Resource (e.g., "PaymentMethod")
1. Create entity in `Domain/Entities/`
2. Add `DbSet<PaymentMethod>` to `FinanceDbContext`
3. Create `IPaymentMethodRepository` interface + `PaymentMethodRepository` implementation
4. Create `IPaymentMethodDomService` + `PaymentMethodDomService`
5. Create `IPaymentMethodAppService` + `PaymentMethodAppService`
6. Create DTOs in `Contracts/Dtos/`
7. Create mapper in `Domain/MapperProfiles/`
8. Register in `Program.cs` (both API and Web apps)
9. Create `PaymentMethodController` in API
10. Create `PaymentMethodCliService` in `Client.Services/`
11. Create controller + views in Web app

### Run Migrations
```powershell
cd src/Infrastructure/FinanceControl.Data
dotnet ef migrations add InitialCreate --startup-project ../../Application/FinanceControl.Web.Api
dotnet ef database update --startup-project ../../Application/FinanceControl.Web.Api
```

### Debug API Locally
- API runs on `https://localhost:5001` (default); Swagger UI: `https://localhost:5001/swagger`
- Web app runs on `https://localhost:7143` (configured in CORS)
- Both require MySQL connection string in `appsettings.Development.json`

## Critical Files

- `src/Application/FinanceControl.Web.Api/Program.cs` — DI container setup
- `src/Infrastructure/FinanceControl.Data/Contexts/FinanceDbContext.cs` — entity mappings
- `src/Crosscutting/FinanceControl.Contracts/` — DTOs & interfaces (contract layer)
- `README.md` — functional requirements (Portuguese)

## Recent Context

- Project targets .NET 8 with `Nullable` enabled
- Primary frontend is ASP.NET Core MVC (Razor Pages planned but currently MVC)
- MySQL backend; no ORM beyond EF Core
- JWT authentication "em análise" (not yet implemented)

## Critical Rule for Code Changes
- Before ANY code changes, file modifications, or implementation work, ALWAYS provide a detailed step-by-step explanation of EXACTLY what you will change. Include: file paths, old content, new content, and rationale. Present this as a clear plan/presentation and WAIT for explicit user approval before executing. Never assume approval - ask every time.
