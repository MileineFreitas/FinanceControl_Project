# Banco de dados — FinanceControl

## Pré-requisitos

- MySQL em execução (local ou remoto)
- Connection string em `src/Application/FinanceControl.Web.Api/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FinanceControl;Uid=root;Password=SUA_SENHA;"
}
```

Ajuste `Uid`, `Password` e `Server` conforme seu ambiente.

## Uma única migration (recomendado)

O projeto usa **uma migration inicial** (`InitialCreate`) que cria todas as tabelas de uma vez.

### Opção A — Automático ao subir a API (mais simples)

1. Crie o banco vazio no MySQL (opcional; o MySQL pode criar ao conectar):

```sql
CREATE DATABASE IF NOT EXISTS FinanceControl CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. Inicie o projeto **FinanceControl.Web.Api** (F5 ou `dotnet run`).

Na primeira execução a API:

- Aplica `dotnet ef database update` automaticamente (`Migrate()`)
- Insere **dados de demonstração** (usuário, conta, categorias, transações)

### Opção B — Linha de comando (manual)

Na raiz do repositório:

```powershell
cd src\Infrastructure\FinanceControl.Data
dotnet ef database update --startup-project ..\..\Application\FinanceControl.Web.Api\FinanceControl.Web.Api.csproj
```

Depois suba a API uma vez para rodar o seed (ou use a Opção A).

## Tabelas criadas

| Tabela | Conteúdo |
|--------|----------|
| `Users` | Usuários |
| `Accounts` | Contas bancárias |
| `Categories` | Categorias (sem receita/despesa) |
| `Transactions` | Lançamentos (tipo receita/despesa + meio de pagamento) |
| `TransactionTypes` | Meios de pagamento sistema (débito, crédito, dinheiro) |

## Dados de demonstração (seed)

Após a primeira carga:

| Item | Valor |
|------|--------|
| Usuário | `demo@financecontrol.local` / senha `demo123` |
| Conta | Conta Principal (id 1) |
| Categorias | Salário, Investimentos, Freelance, Moradia, Alimentação, Transporte, Saúde, Lazer, Educação |
| Transações | 11 lançamentos de exemplo com saldo calculado na conta |

O seed é **idempotente**: se já existirem dados, não duplica categorias nem transações.

## Recriar do zero

```sql
DROP DATABASE IF EXISTS FinanceControl;
CREATE DATABASE FinanceControl CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Em seguida suba a API novamente (Opção A) ou rode `dotnet ef database update`.

## Gerar nova migration (só se o modelo mudar)

```powershell
cd src\Infrastructure\FinanceControl.Data
dotnet ef migrations add NomeDaAlteracao --startup-project ..\..\Application\FinanceControl.Web.Api\FinanceControl.Web.Api.csproj
dotnet ef database update --startup-project ..\..\Application\FinanceControl.Web.Api\FinanceControl.Web.Api.csproj
```
