# FinanceControl_Project

> Sistema web de controle financeiro pessoal — API REST em ASP.NET Core 8 com interface MVC responsiva.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API%20%2B%20MVC-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Entity%20Framework%20Core-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?logo=swagger&logoColor=black)](https://swagger.io/)
[![Tests](https://img.shields.io/badge/Tests-xUnit%20%2B%20Integration-107C10?logo=xunit)](src/Tests/FinanceControl.Tests/)

---

## Índice

- [Domínio do Problema](#-domínio-do-problema)
- [Objetivo](#-objetivo)
- [Visão geral da solução](#-visão-geral-da-solução)
- [Funcionalidades implementadas](#-funcionalidades-implementadas)
- [Requisitos Funcionais (RF)](#-requisitos-funcionais-rf)
- [Requisitos Não Funcionais (RNF)](#-requisitos-não-funcionais-rnf)
- [Tecnologias](#-tecnologias)
- [Estrutura de Pastas](#-1️⃣-estrutura-de-pastas-net-8---arquitetura-em-camadas)
- [O que cada camada faz?](#-o-que-cada-camada-faz)
- [Diagrama de Classes](#-2️⃣-diagrama-de-classes)
- [Diagrama C4](#diagrama-c4)
- [Pacotes necessários](#pacotes-necessários)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e execução](#instalação-e-execução)
- [Configuração](#configuração)
- [Documentação da API](#documentação-da-api)
- [Testes](#testes)
- [Melhorias](#melhorias)
- [Contribuição](#contribuição)
- [Autores](#autores)

---

## 📌 Domínio do Problema

Muitas pessoas possuem dificuldades para organizar suas finanças pessoais, controlar gastos mensais, planejar seus investimentos e visualizar seu saldo de forma estruturada.

O projeto **FinanceControl** propõe a implementação de um sistema responsável por:

- Gerenciar usuários
- Registrar receitas e despesas
- Organizar transações por categorias
- Gerenciar contas e meios de pagamento
- Calcular saldo financeiro
- Gerar resumo mensal e relatórios visuais

A aplicação é composta por uma **API REST** (backend) e um **frontend web** desenvolvido em **HTML, CSS, JavaScript e Razor (CSHTML)**, utilizando **ASP.NET Core MVC**. A interface consome a API via HTTP e oferece suporte a múltiplos idiomas (`pt-BR`, `en-US`, `es-ES`).

---

## 🎯 Objetivo

Desenvolver uma API RESTful utilizando [ASP.NET Core .NET 8](https://dotnet.microsoft.com/apps/aspnet), aplicando boas práticas de **arquitetura em camadas** (inspirada em Clean Architecture), **autenticação** (cookie na interface web; JWT em análise para a API) e **persistência de dados** em banco relacional **Microsoft SQL Server** via Entity Framework Core.

---

## 🔍 Visão geral da solução

A solução é composta por dois projetos executáveis:

| Projeto | Descrição | URL (desenvolvimento) |
|---------|-----------|------------------------|
| **FinanceControl.Web.Api** | API REST com persistência, regras de negócio e Swagger | `https://localhost:7189` |
| **FinanceControl.Web** | Interface MVC (Razor Views) que consome a API | `https://localhost:7143` |

```
┌─────────────────────────────────────────────────────────┐
│  Presentation — FinanceControl.Web + Client.Services    │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP
┌──────────────────────────▼──────────────────────────────┐
│  Application — FinanceControl.Web.Api + Services        │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│  Domain — FinanceControl.Domain + Domain.Services       │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│  Infrastructure — FinanceControl.Data (EF Core + SQL)   │
└─────────────────────────────────────────────────────────┘
         ▲
         │ DTOs, enums, contratos
┌────────┴────────────────────────────────────────────────┐
│  Crosscutting — FinanceControl.Contracts + Culture      │
└─────────────────────────────────────────────────────────┘
```

---

## ✅ Funcionalidades implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Usuários e acesso | ✅ | Registro, login, logout, perfil, alteração de senha, exclusão de conta, revogação de sessões |
| Preferências | ✅ | Moeda, idioma, formato de data e início do mês financeiro |
| Categorias | ✅ | CRUD de categorias de receita e despesa |
| Contas | ✅ | Gerenciamento de contas bancárias/carteiras |
| Meios de pagamento | ✅ | CRUD de formas de pagamento |
| Transações | ✅ | CRUD com filtros por período, categoria, tipo e valor; status pago/pendente |
| Dashboards | ✅ | Visão geral, despesas por categoria, evolução de transações |
| Relatórios | ✅ | Agrupamentos por categoria, meio de pagamento e transações |
| Internacionalização | ✅ | Interface em português, inglês e espanhol |
| JWT na API | 🔄 | Em análise |
| Recuperação de senha | 🔄 | Planejado |
| Transações recorrentes | 🔄 | Planejado |
| Metas e orçamentos | 🔄 | Planejado |
| Exportação PDF/CSV | 🔄 | Em análise |
| Notificações | 🔄 | Planejado |

---

# ✅ REQUISITOS FUNCIONAIS (RF)

## 🔐 1. Usuários e Acesso

**RF01 –** O sistema deve permitir cadastro de usuário. ✅

**RF02 –** O sistema deve permitir autenticação (login). ✅

**RF03 –** O sistema deve permitir logout. ✅

**RF04 –** O sistema deve permitir recuperação de senha via e-mail.

**RF05 –** O sistema deve permitir edição de dados cadastrais do usuário. ✅

**RF06 –** O sistema deve permitir exclusão/inativação de conta. ✅

---

## 🗂 2. Categorias

**RF07 –** O sistema deve permitir cadastro de categorias de receita. ✅

**RF08 –** O sistema deve permitir cadastro de categorias de despesa. ✅

**RF09 –** O sistema deve permitir editar categorias. ✅

**RF10 –** O sistema deve permitir excluir categorias (caso não estejam vinculadas a transações). ✅

**RF11 –** O sistema deve permitir definir categorias padrão no primeiro acesso. ✅

---

## 💰 3. Transações (Receitas e Despesas)

**RF12 –** O sistema deve permitir registrar receitas. ✅

**RF13 –** O sistema deve permitir registrar despesas. ✅

**RF14 –** O sistema deve permitir editar transações. ✅

**RF15 –** O sistema deve permitir excluir transações. ✅

**RF16 –** O sistema deve permitir listar transações por período. ✅

**RF17 –** O sistema deve permitir filtrar transações por: ✅

- Data
- Categoria
- Tipo (receita/despesa)
- Valor

**RF18 –** O sistema deve permitir marcar despesas como pagas/pendentes. ✅

**RF19 –** O sistema deve permitir registrar transações recorrentes (mensais, semanais, etc.).

**RF20 –** O sistema deve permitir anexar observações às transações.

---

## 📊 4. Saldo e Cálculos

**RF21 –** O sistema deve calcular o saldo automaticamente com base nas receitas e despesas. ✅

**RF22 –** O sistema deve exibir saldo atual em tempo real. ✅

**RF23 –** O sistema deve calcular total de receitas por período. ✅

**RF24 –** O sistema deve calcular total de despesas por período. ✅

**RF25 –** O sistema deve calcular economia (receita - despesa) mensal. ✅

---

## 📈 5. Relatórios e Visualizações

**RF26 –** O sistema deve permitir gerar resumo mensal. ✅

**RF27 –** O sistema deve gerar gráficos de: ✅

- Despesas por categoria
- Evolução do saldo
- Comparativo mensal

**RF28 –** O sistema deve permitir exportar relatórios em PDF. *(Em análise)*

**RF29 –** O sistema deve permitir exportar dados em CSV/Excel. *(Em análise)*

**RF30 –** O sistema deve permitir visualizar projeção financeira baseada em médias anteriores.

---

## 🎯 6. Planejamento Financeiro

**RF31 –** O sistema deve permitir definir metas financeiras (ex: economizar R$ 5.000).

**RF32 –** O sistema deve acompanhar o progresso da meta.

**RF33 –** O sistema deve permitir definir orçamento mensal por categoria.

**RF34 –** O sistema deve alertar quando o orçamento estiver próximo do limite.

---

## 🔔 7. Notificações

**RF35 –** O sistema deve enviar notificações de contas próximas do vencimento.

**RF36 –** O sistema deve emitir alerta quando a despesa ultrapassar a receita.

---

# 📑 REQUISITOS NÃO FUNCIONAIS (RNF)

---

## 🏗 1. Arquitetura e Plataforma

**RNF01** – O sistema deve ser uma aplicação web responsiva. ✅

**RNF02** – Deve funcionar nos principais navegadores (Chrome, Edge, Firefox, Safari). ✅

---

## ⚡ 2. Desempenho

**RNF03** – O tempo de resposta para operações comuns não deve ultrapassar 3 segundos.

**RNF04** – O sistema deve suportar pelo menos 300 usuários simultâneos (definir meta).

---

## 🔒 3. Segurança

**RNF05** – As senhas devem ser armazenadas de forma criptografada (hash seguro). ✅

**RNF06** – A comunicação deve utilizar HTTPS. ✅

**RNF07** – O sistema deve garantir que um usuário só visualize seus próprios dados. ✅

**RNF08** – Deve haver controle contra ataques comuns (SQL Injection, XSS, CSRF). *(Parcial — EF Core parametrizado; validação de SecurityStamp nas sessões)*

**RNF09** – O sistema deve implementar controle de sessão com expiração automática. ✅ *(Cookie com sliding expiration de 8h)*

---

## 🗄 4. Banco de Dados

**RNF10** – O sistema deve utilizar banco de dados relacional ou não relacional com integridade de dados garantida. ✅ *(SQL Server + EF Core)*

**RNF11** – Deve haver backup automático diário do banco de dados. *(Depende do provedor — Azure SQL)*

---

## 📈 5. Escalabilidade

**RNF12** – O sistema deve permitir escalabilidade horizontal ou vertical conforme crescimento da base de usuários.

---

## 🎨 6. Usabilidade

**RNF13** – A interface deve ser intuitiva e de fácil navegação. ✅

**RNF14** – O sistema deve seguir princípios básicos de UX/UI. ✅

---

## 🔄 7. Manutenibilidade

**RNF15** – O código deve seguir padrão de arquitetura definida (ex: MVC). ✅ *(Arquitetura em camadas + MVC na apresentação)*

**RNF16** – O sistema deve possuir documentação técnica. ✅ *(README + Swagger)*

**RNF17** – O sistema deve possuir testes automatizados (unitários e/ou integração), utilizando ferramentas como Postman ou Insomnia, além de monitoramento de logs. ✅ *(Testes de integração com xUnit; Swagger para testes manuais)*

---

## 🛠 Tecnologias

| Tecnologia | Justificativa |
| --- | --- |
| **ASP.NET Core .NET 8** | Alta performance e padrão de mercado |
| **Entity Framework Core 8** | Manipulação simples do banco, usando convenções e migrations para gerar/atualizar tabelas |
| **Microsoft SQL Server** | Integração natural com ecossistema Microsoft; suporte a Azure SQL |
| **ASP.NET Core MVC (Razor)** | Interface web com CSHTML, HTML, CSS e JavaScript |
| **Swagger / OpenAPI** | Teste e documentação interativa da API |
| **xUnit + WebApplicationFactory** | Testes de integração automatizados |
| **FluentValidation** | Validação robusta *(planejado)* |

---

## 📁 1️⃣ Estrutura de Pastas (.NET 8 - Arquitetura em Camadas)

Utilizamos uma arquitetura limpa e organizada (inspirada em Clean Architecture, mas simplificada).

```
FinanceControl_Project/
├── .github/
│   └── copilot-instructions.md
├── docs/
├── scripts/
│   ├── generate-localization.ps1
│   └── test-resources.ps1
├── FinanceControl.sln
└── src/
    ├── Application/
    │   ├── FinanceControl.Services/           # AppServices (casos de uso)
    │   │   ├── Accounts/
    │   │   ├── Categories/
    │   │   ├── PaymentMethods/
    │   │   ├── Transactions/
    │   │   └── Users/
    │   └── FinanceControl.Web.Api/            # Controllers REST + Swagger
    │       ├── Controllers/
    │       │   ├── Accounts/
    │       │   ├── Categories/
    │       │   ├── PaymentMethods/
    │       │   ├── Transactions/
    │       │   └── Users/
    │       └── Properties/
    ├── Crosscutting/
    │   ├── FinanceControl.Contracts/          # DTOs, enums e interfaces
    │   │   ├── Dtos/
    │   │   │   ├── Auth/
    │   │   │   ├── Categories/
    │   │   │   ├── Transactions/
    │   │   │   └── Users/
    │   │   ├── Enumerators/
    │   │   │   └── Transactions/
    │   │   └── Interfaces/
    │   │       └── Entities/
    │   └── FinanceControl.Culture/            # Recursos de localização
    ├── Domain/
    │   ├── FinanceControl.Domain/             # Entidades e contratos
    │   │   ├── Entities/
    │   │   │   ├── Accounts/
    │   │   │   ├── Categories/
    │   │   │   ├── PaymentMethods/
    │   │   │   ├── Transactions/
    │   │   │   └── Users/
    │   │   └── Interfaces/
    │   │       ├── AppServices/
    │   │       ├── DomService/
    │   │       └── Repositories/
    │   └── FinanceControl.Domain.Services/    # Regras de domínio
    │       ├── Accounts/
    │       ├── Categories/
    │       ├── PaymentMethods/
    │       ├── Transactions/
    │       └── Users/
    ├── Infrastructure/
    │   └── FinanceControl.Data/               # EF Core, DbContext, repositórios
    │       ├── Contexts/
    │       ├── EntityConfiguration/
    │       │   ├── Accounts/
    │       │   ├── Categories/
    │       │   ├── PaymentMethods/
    │       │   ├── Transactions/
    │       │   └── Users/
    │       ├── Migrations/
    │       └── Repositories/
    │           ├── Accounts/
    │           ├── Categories/
    │           ├── PaymentMethods/
    │           ├── Transactions/
    │           └── Users/
    ├── Presentation/
    │   ├── FinanceControl.Client.Services/    # Cliente HTTP tipado para a API
    │   └── FinanceControl.Web.App/            # MVC — Views, Controllers, wwwroot
    │       ├── Controllers/
    │       ├── Views/
    │       │   ├── Account/
    │       │   ├── Categories/
    │       │   ├── Dashboards/
    │       │   ├── Home/
    │       │   ├── Login/
    │       │   ├── PaymentMethods/
    │       │   ├── Profile/
    │       │   ├── Register/
    │       │   ├── Reports/
    │       │   ├── Shared/
    │       │   └── Transactions/
    │       ├── Resources/                     # pt-BR, en-US, es-ES
    │       └── wwwroot/
    │           ├── css/
    │           └── js/
    └── Tests/
        └── FinanceControl.Tests/              # Testes de integração (xUnit)
```

---

## 📌 O que cada camada faz?

### 🔵 API (`FinanceControl.Web.Api`)

- Controllers REST (Users, Categories, Transactions, Accounts, PaymentMethods)
- Configuração do Swagger
- CORS e injeção de dependências
- Entrada e saída da aplicação via HTTP
- Configuração do JWT *(em análise)*

---

### 🟢 Application (`FinanceControl.Services`)

- Orquestração de casos de uso (AppServices)
- Coordenação entre domínio e infraestrutura
- DTOs de entrada e saída (via `FinanceControl.Contracts`)

---

### 🟡 Domain (`FinanceControl.Domain` + `Domain.Services`)

- Entidades principais (User, Category, Transaction, Account, PaymentMethod)
- Enums e contratos de domínio
- Regras centrais e validações de negócio (DomServices)

---

### 🟣 Infrastructure (`FinanceControl.Data`)

- EF Core e DbContext
- Implementação dos Repositórios
- Migrations e configuração de entidades
- Conexão com **SQL Server**
- Aplicação automática de migrations na inicialização

---

### 🟠 Presentation (`FinanceControl.Web.App` + `Client.Services`)

- Interface MVC com Razor Views (CSHTML)
- Autenticação por cookie
- Localização (pt-BR, en-US, es-ES)
- Dashboards, relatórios e páginas de CRUD
- Cliente HTTP tipado para consumo da API

---

## 🧱 2️⃣ Diagrama de Classes

Modelo de domínio atual do projeto:

---

### 🔹 Usuario (`User`)

- UserId
- UserName
- UserEmail
- Password *(hash)*
- ProfilePhoto
- IsActive
- SecurityStamp
- DateCreated
- Currency, Language, DateFormat, FinancialMonthStartDay *(preferências)*
- Lista de Transacoes, Categories, Accounts, PaymentMethods

---

### 🔹 Transacao (`Transaction`)

- TransactionId
- TransactionDescription
- TransactionValue
- Date
- TransactionTypeKind *(Receita ou Despesa)*
- TransactionStatus *(Pago/Pendente)*
- CategoryId, AccountId, PaymentMethodId, UserId
- CreatedAt, UpdatedAt

---

### 🔹 Categoria (`Category`)

- CategoryId
- CategoryName
- Description
- Icon
- UserId
- IsActive
- DateCreated, UpdatedAt

---

### 🔹 Conta (`Account`)

- AccountId
- Name
- InitialBalance
- CurrentBalance
- UserId
- IsActive
- CreatedAt

---

### 🔹 Meio de Pagamento (`PaymentMethod`)

- PaymentMethodId
- Name
- Icon
- Description
- UserId
- IsActive
- DateCreated, UpdatedAt

---

### Relacionamentos

```
User ──┬── Category
       ├── Account
       ├── PaymentMethod
       └── Transaction ──┬── Category
                         ├── Account
                         └── PaymentMethod
```

---

## DIAGRAMA C4

![Diagrama C4 — Contexto](https://github.com/user-attachments/assets/50df72be-cc82-49ed-89bb-998a561be430)

![Diagrama C4 — Containers](https://github.com/user-attachments/assets/81fa032e-f365-4965-a0e1-333a676490dd)

![Diagrama C4 — Componentes](https://github.com/user-attachments/assets/64d2e2ba-6754-4570-b772-4cd2d9fda546)

---

## Pacotes necessários

Pacotes NuGet utilizados no projeto:

| Pacote | Projeto | Finalidade |
|--------|---------|------------|
| `Microsoft.EntityFrameworkCore` | Web.Api / Data | ORM |
| `Microsoft.EntityFrameworkCore.Design` | Web.Api | Ferramenta de migrations |
| `Microsoft.EntityFrameworkCore.SqlServer` | Web.Api / Data | Provider SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | Web.Api | CLI `dotnet ef` |
| `Swashbuckle.AspNetCore` | Web.Api | Swagger / OpenAPI |
| `Microsoft.AspNetCore.Mvc.Testing` | Tests | Testes de integração |
| `Microsoft.EntityFrameworkCore.InMemory` | Tests | Banco em memória para testes |
| `xunit` | Tests | Framework de testes |

**Ferramenta global EF Core:**

```bash
dotnet tool install --global dotnet-ef
```

**Aplicar migrations:**

```bash
dotnet ef database update \
  --project src/Infrastructure/FinanceControl.Data \
  --startup-project src/Application/FinanceControl.Web.Api
```

**Configuração:** definir a connection string em `appsettings.json` ou via [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) — não commite credenciais reais no repositório.

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express ou instância remota/Azure SQL)
- [Git](https://git-scm.com/)
- IDE recomendada: [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

---

## Instalação e execução

### 1. Clonar o repositório

```bash
git clone https://github.com/MileineFreitas/FinanceControl_Project.git
cd FinanceControl_Project
```

### 2. Restaurar dependências

```bash
dotnet restore FinanceControl.sln
```

### 3. Configurar o banco de dados

Defina a connection string (veja [Configuração](#configuração)) e aplique as migrations (ou inicie a API — migrations são aplicadas automaticamente em desenvolvimento).

### 4. Executar a API

```bash
dotnet run --project src/Application/FinanceControl.Web.Api
```

API disponível em `https://localhost:7189` · Swagger em `/swagger`

### 5. Executar a interface web

Em outro terminal:

```bash
dotnet run --project src/Presentation/FinanceControl.Web.App
```

Acesse `https://localhost:7143` no navegador.

### Execução via Visual Studio

1. Abra `FinanceControl.sln`
2. Configure **Multiple Startup Projects**: `FinanceControl.Web.Api` e `FinanceControl.Web`
3. Pressione **F5**

---

## Configuração

### API — connection string

Edite `src/Application/FinanceControl.Web.Api/appsettings.json` ou use User Secrets:

```bash
cd src/Application/FinanceControl.Web.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=SEU_SERVIDOR;Database=FinanceControl;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Web — URL da API

Em `src/Presentation/FinanceControl.Web.App/appsettings.json`:

```json
{
  "ApiClient": {
    "BaseUrl": "https://localhost:7189",
    "TimeoutSeconds": 100
  }
}
```

A URL deve corresponder à porta configurada em `launchSettings.json` da API.

---

## Documentação da API

Com a API em execução, acesse:

```
https://localhost:7189/swagger
```

| Recurso | Rota base | Operações |
|---------|-----------|-----------|
| Usuários | `/User` | login, register, CRUD, exclusão de conta, revogação de sessões |
| Categorias | `/api/Category` | listar, obter, criar, editar, excluir |
| Transações | `/api/Transaction` | listar (com filtros), obter, criar, editar, excluir |
| Contas | `/api/Account` | listar, obter, criar, editar, excluir |
| Meios de pagamento | `/api/PaymentMethods` | listar, obter, criar, editar, excluir |

---

## Testes

Execute a suíte de testes de integração:

```bash
dotnet test src/Tests/FinanceControl.Tests
```

Cobertura atual: CRUD de usuários, categorias, contas e transações; atualização de senha e foto de perfil. Utiliza `WebApplicationFactory` com banco InMemory.

Também é possível testar manualmente via **Swagger**, **Postman** ou **Insomnia**.

---

## Melhorias

Itens em andamento e planejados para próximas iterações:

### Autenticação e segurança

- Aplicar token **JWT** a partir do login do usuário na API *(em análise)*
- Implementar **recuperação de senha** por e-mail

**Principais responsabilidades do fluxo de autenticação (`UserController` / `LoginController`):**

- **Registro (Sign-up/Register):** cria novos usuários no sistema. ✅
- **Login (Autenticação):** valida credenciais e cria sessão por cookie na web; emissão de JWT planejada. ✅ *(cookie)* / 🔄 *(JWT)*
- **Logout:** finaliza a sessão do usuário ou invalida o token. ✅
- **Recuperação de Senha:** gerencia a lógica para redefinir senhas. 🔄

### Regras de negócio

- Refinar e expandir **regras de cálculos** financeiros (projeções, metas, orçamentos)
- Implementar **transações recorrentes**
- Adicionar **observações** nas transações

### Relatórios e exportação

- Exportação de relatórios em **PDF** *(em análise — ver `docs/pdf-generator`)*
- Exportação de dados em **CSV/Excel** *(em análise)*

### Qualidade

- Adicionar **FluentValidation** para validação centralizada
- Expandir cobertura de **testes unitários**
- Configurar **CI/CD** em `.github/workflows`

---

## Contribuição

Contribuições são bem-vindas. Para propor alterações:

1. Faça um fork do repositório
2. Crie uma branch descritiva (`feature/nome-da-feature`)
3. Commit suas alterações com mensagens claras
4. Abra um Pull Request descrevendo o que foi feito e como testar

Mantenha o padrão de arquitetura em camadas e adicione testes de integração para novos fluxos de API.

---

## Autores

| | Mileine Freitas | Julio Mattos |
|---|-----------------|--------------|
| **GitHub** | [@MileineFreitas](https://github.com/MileineFreitas) | [@JulioMattoos](https://github.com/JulioMattoos) |
| **LinkedIn** | — | [in/juliodemattosmanoel](https://www.linkedin.com/in/juliodemattosmanoel) |

Repositório: [FinanceControl_Project](https://github.com/MileineFreitas/FinanceControl_Project)

---

<p align="center">
  Desenvolvido com ASP.NET Core 8
</p>
