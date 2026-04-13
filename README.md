# FinanceControl_Project

## 📌 Domínio do Problema

Muitas pessoas possuem dificuldades para organizar suas finanças pessoais, controlar gastos mensais, planejar seus investimentos e visualizar seu saldo de forma estruturada.
O projeto FinanceControl API propõe a implementação de um sistema backend responsável por:

Gerenciar usuários

Registrar receitas e despesas

Organizar transações por categorias

Calcular saldo financeiro

Gerar resumo mensal.

A aplicação será consumida por um frontend desenvolvido em HTML, CSS e JavaScript (analisando utilização de ANGULAR), podendo utilizar CSHTML se necessário.

## 🎯 Objetivo

Desenvolver uma API RESTful utilizando [ASP.NET](http://asp.net/) Core .NET 8, aplicando boas práticas de arquitetura em camadas, autenticação JWT(em analise) e persistência de dados em banco relacional MySQL.

# ✅ REQUISITOS FUNCIONAIS (RF)

## 🔐 1. Usuários e Acesso
**RF01 –** O sistema deve permitir cadastro de usuário.

**RF02 –** O sistema deve permitir autenticação (login).

**RF03 –** O sistema deve permitir logout.

**RF04 –** O sistema deve permitir recuperação de senha via e-mail.

**RF05 –** O sistema deve permitir edição de dados cadastrais do usuário.

**RF06 –** O sistema deve permitir exclusão/inativação de conta.

---

## 🗂 2. Categorias

**RF07 –** O sistema deve permitir cadastro de categorias de receita.

**RF08 –** O sistema deve permitir cadastro de categorias de despesa.

**RF09 –** O sistema deve permitir editar categorias.

**RF10 –** O sistema deve permitir excluir categorias (caso não estejam vinculadas a transações).

**RF11 –** O sistema deve permitir definir categorias padrão no primeiro acesso.

---

## 💰 3. Transações (Receitas e Despesas)

**RF12 –** O sistema deve permitir registrar receitas.

**RF13 –** O sistema deve permitir registrar despesas.

**RF14 –** O sistema deve permitir editar transações.

**RF15 –** O sistema deve permitir excluir transações.

**RF16 –** O sistema deve permitir listar transações por período.

**RF17 –** O sistema deve permitir filtrar transações por:

- Data
- Categoria
- Tipo (receita/despesa)
- Valor

**RF18 –** O sistema deve permitir marcar despesas como pagas/pendentes.

**RF19 –** O sistema deve permitir registrar transações recorrentes (mensais, semanais, etc.).

**RF20 –** O sistema deve permitir anexar observações às transações.

---

## 📊 4. Saldo e Cálculos

**RF21 –** O sistema deve calcular o saldo automaticamente com base nas receitas e despesas.

**RF22 –** O sistema deve exibir saldo atual em tempo real.

**RF23 –** O sistema deve calcular total de receitas por período.

**RF24 –** O sistema deve calcular total de despesas por período.

**RF25 –** O sistema deve calcular economia (receita - despesa) mensal.

---

## 📈 5. Relatórios e Visualizações

**RF26 –** O sistema deve permitir gerar resumo mensal.

**RF27 –** O sistema deve gerar gráficos de:

- Despesas por categoria
- Evolução do saldo
- Comparativo mensal

**RF28 –** O sistema deve permitir exportar relatórios em PDF. (Analisando)

**RF29 –** O sistema deve permitir exportar dados em CSV/Excel.(Analisando)

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

# 🏗 1. Arquitetura e Plataforma

**RNF01** – O sistema deve ser uma aplicação web responsiva.

**RNF02** – Deve funcionar nos principais navegadores (Chrome, Edge, Firefox, Safari).

---

# ⚡ 2. Desempenho

**RNF03** – O tempo de resposta para operações comuns não deve ultrapassar 3 segundos.

**RNF04** – O sistema deve suportar pelo menos 300 usuários simultâneos (definir meta).

---

# 🔒 3. Segurança

**RNF05** – As senhas devem ser armazenadas de forma criptografada (hash seguro).

**RNF06** – A comunicação deve utilizar HTTPS.

**RNF07** – O sistema deve garantir que um usuário só visualize seus próprios dados.

**RNF08** – Deve haver controle contra ataques comuns (SQL Injection, XSS, CSRF).

**RNF09** – O sistema deve implementar controle de sessão com expiração automática.

---

# 🗄 4. Banco de Dados

**RNF10** – O sistema deve utilizar banco de dados relacional ou não relacional com integridade de dados garantida.

**RNF11** – Deve haver backup automático diário do banco de dados.

---

# 📈 5. Escalabilidade

**RNF12** – O sistema deve permitir escalabilidade horizontal ou vertical conforme crescimento da base de usuários.

---

# 🎨 6. Usabilidade

**RNF13** – A interface deve ser intuitiva e de fácil navegação.

**RNF14** – O sistema deve seguir princípios básicos de UX/UI.

---

# 🔄 7. Manutenibilidade

**RNF15** – O código deve seguir padrão de arquitetura definida (ex: MVC).

**RNF16** – O sistema deve possuir documentação técnica.

**RNF17** – O sistema deve possuir testes automatizados (unitários e/ou integração), utilizando ferramentas como Postman ou Insomnia, além de monitoramento de logs.

---

## 🛠 Tecnologias

| Tecnologia | Justificativa |
| --- | --- |
| **ASP.NET Core .NET 8** | Alta performance e padrão de mercado |
| **Entity Framework Core** | Manipulação simples do banco, usando convenções para gerar tabelas no banco. |
| **SQL Server** | Integração natural com ecossistema Microsoft |
| **FluentValidation** | Validação robusta |
| **Swagger** | Teste e documentação |

---

- Estrutura de Pastas
    
    # 📁 1️⃣ Estrutura de Pastas (.NET 8 - Arquitetura em Camadas)
    
    Vamos usar uma arquitetura limpa e organizada (inspirada em Clean Architecture, mas simplificada).
    
    ```
    FinanceControl_Project/
    ├── .github/
    │   └── workflows/
    ├── FinanceControl.slnx
    └── src/
        ├── Application/
        │   ├── FinanceControl.API/
        │   │   ├── Configurations/
        │   │   ├── Controllers/
        │   │   │   ├── Categories/
        │   │   │   ├── Transactions/
        │   │   │   └── Users/
        │   │   └── Properties/
        │   └── FinanceControl.Application/
        │       ├── Categories/
        │       ├── Transactions/
        │       └── Users/
        ├── Crosscutting/
        │   └── FinanceControl.Crosscutting/
        │       ├── Dtos/
        │       │   ├── Auth/
        │       │   ├── Categories/
        │       │   ├── Transactions/
        │       │   └── Users/
        │       ├── Enumerators/
        │       │   └── Transactions/
        │       └── Interfaces/
        │           └── Entities/
        ├── Domain/
        │   ├── FinanceControl.Domain/
        │   │   ├── Entities/
        │   │   │   ├── Categories/
        │   │   │   ├── Transactions/
        │   │   │   └── Users/
        │   │   ├── Enums/
        │   │   ├── Interfaces/
        │   │   │   ├── AppServices/
        │   │   │   ├── DomService/
        │   │   │   └── Repositories/
        │   │   └── Properties/
        │   └── FinanceControl.Domain.Services/
        │       ├── Categories/
        │       ├── Transactions/
        │       └── Users/
        ├── Infrastructure/
        │   └── FinanceControl.Infrastructure/
        │       ├── Contexts/
        │       ├── EntityConfiguration/
        │       │   ├── Categories/
        │       │   ├── Transactions/
        │       │   └── Users/
        │       ├── Migrations/
        │       └── Repositories/
        │           ├── Categories/
        │           ├── Transactions/
        │           └── Users/
        ├── Presentation/
        │   └── FinanceControl.Blazor/
        │       ├── Layout/
        │       ├── Pages/
        │       │   ├── Categorias/
        │       │   ├── Conta/
        │       │   ├── Dashboards/
        │       │   ├── Home/
        │       │   ├── Login/
        │       │   ├── Messages/
        │       │   ├── Perfil/
        │       │   ├── Register/
        │       │   ├── Relatorios/
        │       │   ├── Shared/
        │       │   └── Transactions/
        │       ├── Properties/
        │       ├── Services/
        │       └── wwwroot/
        │           ├── css/
        │           ├── js/
        │           ├── lib/
        │           └── sample-data/
        └── Tests/
            └── FinanceControl.Tests/
    ```
    
    ---
    
    ## 📌 O que cada camada faz?
    
    ### 🔵 API
    
    - Controllers
    - Configuração do JWT
    - Configuração do Swagger
    - Entrada e saída da aplicação
    
    ---
    
    ### 🟢 Application
    
    - Regras de negócio
    - Services
    - DTOs (entrada e saída)
    - Interfaces
    
    ---
    
    ### 🟡 Domain
    
    - Entidades principais
    - Enums
    - Regras centrais
    
    ---
    
    ### 🟣 Infrastructure
    
    - EF Core
    - DbContext
    - Implementação dos Repositórios
    - Conexão com MySQL
    - Diagrama de Classes
    
    # 🧱 2️⃣ Diagrama de Classes
    
    Aqui está o modelo ideal para o seu projeto:
    
    ---
    
    ## 📌 Entidades Principais
    
    ### 🔹 Usuario
    
    - Id
    - Nome
    - Email
    - SenhaHash
    - DataCriacao
    - Lista de Transacoes
    
    ---
    
    ### 🔹 Transacao
    
    - Id
    - Descricao
    - Valor
    - Data
    - Tipo (Receita ou Despesa)
    - CategoriaId
    - UsuarioId
    
    ---
    
    ### 🔹 Categoria
    
    - Id
    - Nome
    - UsuarioId
    
    ---


<img width="457" height="493" alt="image" src="https://github.com/user-attachments/assets/5decfd96-fbf9-4eea-8a49-37506f7faa94" />

<img width="448" height="389" alt="image" src="https://github.com/user-attachments/assets/d1fdb2dc-38da-443b-a97a-0ba31a91e4ed" />

<img width="2313" height="829" alt="image" src="https://github.com/user-attachments/assets/1fac45a9-04d1-40a2-b231-6e2269896c3d" />




    ---




Pacotes necessários:

EntityFrameworkCore → baixar ferramenta.

EntityFrameworkCore.Designe

Pomelo

Corrigir conexão do banco no app.settings

## Melhorias:

Aplicar token JWT a partir do login do usuario.

Principais Responsabilidades do AuthController

- **Registro (Sign-up/Register):** Cria novos usuários no sistema.
- **Login (Autenticação):** Valida as credenciais do usuário e cria uma sessão ou emite um token de acesso (JWT, OAuth).
- **Logout:** Finaliza a sessão do usuário ou invalida o token.
- **Recuperação de Senha:** Gerencia a lógica para redefinir senhas

### Implementar regra de cálculos.
