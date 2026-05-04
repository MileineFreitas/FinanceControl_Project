# Mapa da estrutura do repositório

Este ficheiro ajuda a alinhar a **árvore esperada** com o que está **no disco** após confusões no Solution Explorer (Folder View, `.slnx`, projetos unloaded).

## O que não se perdeu

O código em `src/` está versionado no Git. A camada **Application / Domain / Infrastructure / Crosscutting / Tests** está presente, com **API** em `Application/FinanceControl.API` e **apresentação** em `Presentation/FinanceControl.Web`.

## Diferenças em relação a um diagrama antigo

| Diagrama / memória | Repositório actual |
|--------------------|--------------------|
| `Presentation/FinanceControl.**Blazor**` | **`FinanceControl.Web`** (ASP.NET Core **Razor Pages**, não Blazor) |
| `Infrastructure/.../EntityConfiguration/` | Configuração EF em **`Contexts/FinanceDbContext.cs`** (`OnModelCreating`); não há pasta `EntityConfiguration` separada |
| Vários `Enums` em `Domain/Enums` | Enums podem estar em **Domain** e em **Crosscutting/Enumerators** (ex.: `TransactionStatus`) |
| `API/Controllers/...` só 3 pastas | Existem também **Accounts**, **TransactionTypes**, além de Categories, Transactions, Users |
| `Application/FinanceControl.Application` com 3 pastas | Inclui ficheiros de serviço; a árvore exacta segue o que está no Explorer de ficheiros |

## Abrir a solução no Visual Studio

1. Abre **`FinanceControl.sln`** (formato clássico, amplamente suportado).
2. No **Solution Explorer**, usa **Solution View** (não *Folder View*).
3. Os projectos estão agrupados em pastas de solução: `src\Application`, `src\Domain`, etc.

## Comandos úteis

```bash
# Compilar tudo
dotnet build FinanceControl.sln

# Testes
dotnet test src/Tests/FinanceControl.Tests/FinanceControl.Tests.csproj
```

## Arranque (F5)

- **Site (UI):** `FinanceControl.Web`
- **API:** `FinanceControl.Web.Api`  
- Não uses **FinanceControl.Tests** como startup (é só para `dotnet test` / Test Explorer).
