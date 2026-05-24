# Cronograma — Remoção de `CancellationToken` do FinanceControl

**Status:** Concluído (opção **A** aprovada em 24/05/2026)  
**Data do levantamento:** 24/05/2026  
**Escopo solicitado:** remover todos os `CancellationToken` do projeto  
**Motivo informado:** implementação futura de JWT  

---

## Aprovação

| Campo | Valor |
|-------|-------|
| Aprovado por | _________________________ |
| Data | _________________________ |
| Observações | _________________________ |

> Responda **“aprovado”** (ou indique ajustes) para que a alteração seja aplicada no código.

---

## Observação importante (leia antes de aprovar)

`CancellationToken` e **JWT** resolvem problemas **diferentes** e **não se substituem**:

| Recurso | Função |
|---------|--------|
| **JWT** | Autenticação e autorização — quem pode acessar a API |
| **CancellationToken** | Cancelamento cooperativo de operações async quando a requisição HTTP é interrompida (usuário fecha aba, timeout, etc.) |

O projeto **ainda não possui JWT** implementado (`ESTRUTURA.md` e `.github/copilot-instructions.md` indicam “em análise”).  
Remover `CancellationToken` **não implementa JWT**; apenas simplifica assinaturas e deixa de propagar cancelamento até EF Core e `HttpClient`.

**Recomendação técnica:** manter `CancellationToken` mesmo após JWT. Se a decisão for manter a remoção, este cronograma cobre o trabalho de forma segura e verificável.

---

## Resumo quantitativo

| Métrica | Valor |
|---------|-------|
| Arquivos `.cs` afetados | **43** |
| Arquivos de documentação | **2** (`ESTRUTURA.md`, `.github/copilot-instructions.md`) |
| **Total de arquivos** | **45** |
| Ocorrências aproximadas de `cancellationToken` / `CancellationToken` | **~500+** (remoção mecânica em assinaturas e chamadas) |
| Entidades / módulos de negócio | Accounts, Categories, Transactions, TransactionTypes, Users |
| JWT incluído neste cronograma? | **Não** — escopo separado (ver Fase 0 opcional) |

---

## Padrão de alteração (igual em todo o código)

### Assinaturas

```csharp
// ANTES
Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

// DEPOIS
Task<CategoryDto?> GetByIdAsync(int id);
```

```csharp
// ANTES (controller MVC/API)
public async Task<IActionResult> Index(string? busca, CancellationToken cancellationToken)

// DEPOIS
public async Task<IActionResult> Index(string? busca)
```

### Chamadas EF Core / HttpClient

```csharp
// ANTES
await query.ToListAsync(cancellationToken);
await context.SaveChangesAsync(cancellationToken);
httpClient.GetAsync("api/Category", cancellationToken);

// DEPOIS
await query.ToListAsync();
await context.SaveChangesAsync();
httpClient.GetAsync("api/Category");
```

### Testes (argumento nomeado)

```csharp
// ANTES (IntegrationSeed.cs)
await db.Accounts.AnyAsync(a => a.AccountId == SeedIds.DefaultAccount, cancellationToken: default);

// DEPOIS
await db.Accounts.AnyAsync(a => a.AccountId == SeedIds.DefaultAccount);
```

**Comportamento após a mudança:** operações async continuam funcionando; apenas deixam de ser canceláveis quando o cliente desconecta.

---

## Cronograma por fases

Ordem sugerida: **de baixo para cima** (interfaces → implementações → controllers → docs → build/test).

| Fase | Camada | Arquivos | Estimativa | Risco |
|------|--------|----------|------------|-------|
| **0** *(opcional, fora do escopo atual)* | JWT | Novos arquivos + `Program.cs` API | Projeto à parte | Médio |
| **1** | Domain — interfaces `I*Repository` | 5 | Baixo | Baixo |
| **2** | Domain — interfaces `I*AppService` | 5 | Baixo | Baixo |
| **3** | Infrastructure — `*Repository.cs` | 5 | Baixo | Baixo |
| **4** | Application — `*AppService.cs` | 5 | Baixo | Baixo |
| **5** | Web.Api — controllers | 5 | Baixo | Baixo |
| **6** | Client.Services — interfaces | 6 | Baixo | Baixo |
| **7** | Client.Services — `IFinanceControlApiClient` + integrados | 6 | Baixo | Baixo |
| **8** | Web.App — controllers MVC | 6 | Baixo | Baixo |
| **9** | Tests | 1 | Baixo | Baixo |
| **10** | Documentação | 2 | Baixo | Nenhum |
| **11** | Validação | `dotnet build` + `dotnet test` | — | — |

**Estimativa total de implementação:** 1 sessão de alteração mecânica + build/test (~15–30 min após aprovação).

---

## Fase 0 (opcional) — JWT — **não incluída nesta remoção**

Se desejar JWT no mesmo pacote de trabalho, seria um cronograma **adicional** com itens como:

- Pacote `Microsoft.AspNetCore.Authentication.JwtBearer`
- Configuração em `appsettings` (Issuer, Audience, Secret/Key)
- `AddAuthentication` / `AddJwtBearer` em `Program.cs` (API)
- Emissão de token no login (`UserController` / `UserAppService`)
- `[Authorize]` nos endpoints protegidos
- Envio do token no `HttpClient` do `FinanceControl.Client.Services` (header `Authorization: Bearer ...`)

**Confirme na aprovação** se deseja apenas remoção do `CancellationToken` ou também a Fase 0 (JWT).

---

## Fase 1 — Domain: interfaces de repositório (5 arquivos)

| # | Arquivo | Métodos afetados (remover parâmetro `CancellationToken`) |
|---|---------|----------------------------------------------------------|
| 1.1 | `src/Domain/FinanceControl.Domain/Interfaces/Repositories/Accounts/IAccountRepository.cs` | Todos os métodos async |
| 1.2 | `src/Domain/FinanceControl.Domain/Interfaces/Repositories/Categories/ICategoryRepository.cs` | Todos os métodos async |
| 1.3 | `src/Domain/FinanceControl.Domain/Interfaces/Repositories/Transactions/ITransactionRepository.cs` | Todos os métodos async |
| 1.4 | `src/Domain/FinanceControl.Domain/Interfaces/Repositories/TransactionTypes/ITransactionTypeRepository.cs` | Todos os métodos async |
| 1.5 | `src/Domain/FinanceControl.Domain/Interfaces/Repositories/Users/IUserRepository.cs` | Todos os métodos async |

---

## Fase 2 — Domain: interfaces de AppService (5 arquivos)

| # | Arquivo |
|---|---------|
| 2.1 | `src/Domain/FinanceControl.Domain/Interfaces/AppServices/Accounts/IAccountAppService.cs` |
| 2.2 | `src/Domain/FinanceControl.Domain/Interfaces/AppServices/Categories/ICategoryAppService.cs` |
| 2.3 | `src/Domain/FinanceControl.Domain/Interfaces/AppServices/Transactions/ITransactionAppService.cs` |
| 2.4 | `src/Domain/FinanceControl.Domain/Interfaces/AppServices/TransactionTypes/ITransactionTypeAppService.cs` |
| 2.5 | `src/Domain/FinanceControl.Domain/Interfaces/AppServices/Users/IUserAppService.cs` |

---

## Fase 3 — Infrastructure: repositórios EF Core (5 arquivos)

| # | Arquivo | Alterações típicas |
|---|---------|-------------------|
| 3.1 | `src/Infrastructure/FinanceControl.Data/Repositories/Accounts/AccountRepository.cs` | Assinaturas + `*Async()` sem token |
| 3.2 | `src/Infrastructure/FinanceControl.Data/Repositories/Categories/CategoryRepository.cs` | Idem |
| 3.3 | `src/Infrastructure/FinanceControl.Data/Repositories/Transactions/TransactionRepository.cs` | Idem |
| 3.4 | `src/Infrastructure/FinanceControl.Data/Repositories/TransactionTypes/TransactionTypeRepository.cs` | Idem |
| 3.5 | `src/Infrastructure/FinanceControl.Data/Repositories/Users/UserRepository.cs` | Idem |

---

## Fase 4 — Application: AppServices (5 arquivos)

| # | Arquivo |
|---|---------|
| 4.1 | `src/Application/FinanceControl.Services/Accounts/AccountAppService.cs` |
| 4.2 | `src/Application/FinanceControl.Services/Categories/CategoryAppService.cs` |
| 4.3 | `src/Application/FinanceControl.Services/Transactions/TransactionAppService.cs` |
| 4.4 | `src/Application/FinanceControl.Services/TransactionTypes/TransactionTypeAppService.cs` |
| 4.5 | `src/Application/FinanceControl.Services/Users/UserAppService.cs` |

---

## Fase 5 — Web.Api: controllers REST (5 arquivos)

| # | Arquivo | Actions afetadas |
|---|---------|------------------|
| 5.1 | `src/Application/FinanceControl.Web.Api/Controllers/Accounts/AccountController.cs` | Get, GetById, Post, Put, Delete |
| 5.2 | `src/Application/FinanceControl.Web.Api/Controllers/Categories/CategoryController.cs` | Get, GetById, Post, RegisterCategory, Put, Delete |
| 5.3 | `src/Application/FinanceControl.Web.Api/Controllers/Transactions/TransactionController.cs` | CRUD |
| 5.4 | `src/Application/FinanceControl.Web.Api/Controllers/TransactionTypes/TransactionTypesController.cs` | CRUD |
| 5.5 | `src/Application/FinanceControl.Web.Api/Controllers/Users/UserController.cs` | Login, Register, listagem, etc. |

---

## Fase 6 — Client.Services: interfaces (6 arquivos)

| # | Arquivo |
|---|---------|
| 6.1 | `src/Presentation/FinanceControl.Client.Services/Interfaces/Accounts/IAccountCliService.cs` |
| 6.2 | `src/Presentation/FinanceControl.Client.Services/Interfaces/Categories/ICategoryCliService.cs` |
| 6.3 | `src/Presentation/FinanceControl.Client.Services/Interfaces/Transactions/ITransactionCliService.cs` |
| 6.4 | `src/Presentation/FinanceControl.Client.Services/Interfaces/TransactionTypes/ITransactionTypeCliService.cs` |
| 6.5 | `src/Presentation/FinanceControl.Client.Services/Interfaces/Users/IUserCliService.cs` |
| 6.6 | `src/Presentation/FinanceControl.Client.Services/Interfaces/IFinanceControlApiClient.cs` |

---

## Fase 7 — Client.Services: implementações HTTP (6 arquivos)

| # | Arquivo |
|---|---------|
| 7.1 | `src/Presentation/FinanceControl.Client.Services/Integrated/Accounts/AccountCliService.cs` |
| 7.2 | `src/Presentation/FinanceControl.Client.Services/Integrated/Categories/CategoryCliService.cs` |
| 7.3 | `src/Presentation/FinanceControl.Client.Services/Integrated/Transactions/TransactionCliService.cs` |
| 7.4 | `src/Presentation/FinanceControl.Client.Services/Integrated/TransactionTypes/TransactionTypeCliService.cs` |
| 7.5 | `src/Presentation/FinanceControl.Client.Services/Integrated/Users/UserCliService.cs` |
| 7.6 | `src/Presentation/FinanceControl.Client.Services/Integrated/FinanceControlApiClient.cs` |

---

## Fase 8 — Web.App: controllers MVC (6 arquivos)

| # | Arquivo | Observação |
|---|---------|------------|
| 8.1 | `src/Presentation/FinanceControl.Web.App/Controllers/Auth/LoginController.cs` | Login |
| 8.2 | `src/Presentation/FinanceControl.Web.App/Controllers/Categories/CategoriesController.cs` | Inclui métodos privados `LoadListAsync`, `ReadErrorAsync` |
| 8.3 | `src/Presentation/FinanceControl.Web.App/Controllers/Home/HomeController.cs` | Dashboard |
| 8.4 | `src/Presentation/FinanceControl.Web.App/Controllers/Register/RegisterController.cs` | Registo |
| 8.5 | `src/Presentation/FinanceControl.Web.App/Controllers/Transactions/TransactionsController.cs` | Maior volume de métodos |
| 8.6 | `src/Presentation/FinanceControl.Web.App/Controllers/TransactionTypes/TransactionTypesController.cs` | CRUD UI |

**Não alterados neste escopo** (sem `CancellationToken hoje):**  
`HealthController`, `ProfileController`, `AccountController` (Web.App), etc.

---

## Fase 9 — Testes (1 arquivo)

| # | Arquivo | Alteração |
|---|---------|-----------|
| 9.1 | `src/Tests/FinanceControl.Tests/Integration/IntegrationSeed.cs` | Remover `cancellationToken: default` em `AnyAsync` |

Testes de integração (`AccountCrudTests`, `CategoryCrudTests`, `TransactionCrudTests`) **não** referenciam `CancellationToken` diretamente — devem continuar passando após o build.

---

## Fase 10 — Documentação (2 arquivos)

| # | Arquivo | Alteração proposta |
|---|---------|-------------------|
| 10.1 | `ESTRUTURA.md` | Remover item 2 das convenções (“Async com CancellationToken”); ajustar texto para “operações I/O assíncronas” sem mencionar token |
| 10.2 | `.github/copilot-instructions.md` | Remover referência a `CancellationToken` na regra Async/await |

**Opcional (não listado no grep):** adicionar nota no `README.md` apenas se desejar documentar a decisão arquitetural.

---

## Fase 11 — Validação pós-alteração

```powershell
dotnet build FinanceControl.sln
dotnet test src/Tests/FinanceControl.Tests/FinanceControl.Tests.csproj
```

Critérios de aceite:

- [ ] Build da solução sem erros
- [ ] Testes de integração verdes
- [ ] `rg CancellationToken` na pasta `src/` retorna **zero** ocorrências
- [ ] Swagger/API e site MVC funcionam manualmente (smoke test)

---

## O que **não** será alterado

| Item | Motivo |
|------|--------|
| `*DomService.cs` | Já não usam `CancellationToken` |
| Entidades, DTOs, Mappers, EF Configurations | Sem token hoje |
| Migrations / banco | Sem relação |
| Implementação JWT | Escopo separado (Fase 0 opcional) |
| `README.md` | Sem menção a `CancellationToken` no levantamento |

---

## Riscos e impactos

| Impacto | Severidade | Detalhe |
|---------|------------|---------|
| Perda de cancelamento em requisições longas | Baixa | Consultas grandes podem seguir até o fim se o cliente desconectar |
| Compatibilidade de API | Nenhuma | Contrato HTTP inalterado (parâmetro nunca era query/body) |
| Regressão funcional | Baixa | Mudança mecânica; mitigada por `dotnet test` |
| Confusão JWT vs cancelamento | Informativo | JWT exigirá trabalho adicional independente |

---

## Checklist de aprovação rápida

Marque o que deseja executar:

- [x] **A** — Apenas remover `CancellationToken` (Fases 1–11)
- [ ] **B** — Remover `CancellationToken` **e** implementar JWT (Fase 0 + Fases 1–11)
- [ ] **C** — Não prosseguir; manter `CancellationToken` e planejar JWT à parte

---

## Próximo passo

Após sua aprovação (**A**, **B** ou **C** com ajustes), a alteração será aplicada conforme este cronograma, sem commits automáticos (a menos que você solicite).
