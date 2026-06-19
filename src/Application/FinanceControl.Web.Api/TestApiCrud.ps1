<#
.SYNOPSIS
    Smoke test dos CRUDs da Finance Control API (HTTP localhost).
.DESCRIPTION
    Inicie a API em outro terminal:  dotnet run --launch-profile http
    Depois:  .\TestApiCrud.ps1
    Opcional: .\TestApiCrud.ps1 -BaseUrl "http://localhost:5056"
#>
param(
    [string] $BaseUrl = "http://localhost:5056"
)

$ErrorActionPreference = "Stop"

function Invoke-Api {
    param(
        [string] $Method,
        [string] $Path,
        [object] $Body = $null
    )
    $uri = "$BaseUrl$Path"
    $params = @{
        Uri             = $uri
        Method          = $Method
        ContentType     = "application/json"
        UseBasicParsing = $true
    }
    if ($null -ne $Body) {
        $params.Body = ($Body | ConvertTo-Json -Compress -Depth 6)
    }
    Write-Host "`n>>> $Method $Path" -ForegroundColor Cyan
    try {
        $r = Invoke-WebRequest @params
        Write-Host "    Status: $($r.StatusCode)" -ForegroundColor Green
        if ($r.Content) {
            try {
                $r.Content | ConvertFrom-Json | ConvertTo-Json -Depth 8
            }
            catch {
                Write-Host $r.Content
            }
        }
        elseif ($r.StatusCode -eq 204) {
            Write-Host "    (sem corpo — ex.: 204 No Content)" -ForegroundColor DarkGray
        }
        return $r
    }
    catch {
        Write-Host "    ERRO: $_" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
            Write-Host $reader.ReadToEnd()
        }
        throw
    }
}

Write-Host "=== Finance Control — teste de CRUD ===" -ForegroundColor Yellow
Write-Host "BaseUrl: $BaseUrl (API deve estar rodando)`n"

# 1) Tipos
Invoke-Api GET "/api/TransactionTypes" | Out-Null

# 2) Contas
Invoke-Api GET "/api/Account" | Out-Null
$accResp = Invoke-Api POST "/api/Account" @{
    name           = "Conta PS1 $(Get-Date -Format 'HHmmss')"
    initialBalance = 50
    userId         = $null
}
$accJson = $accResp.Content | ConvertFrom-Json
$newAccountId = $accJson.accountId
if (-not $newAccountId) { $newAccountId = $accJson.AccountId }
Write-Host "    Nova conta id: $newAccountId" -ForegroundColor DarkGray

# 3) Usuário (para FK em transação)
$usersResp = Invoke-Api GET "/user"
$users = $usersResp.Content | ConvertFrom-Json
$userId = $null
if ($null -ne $users) {
    if ($users -is [System.Array]) {
        if ($users.Count -gt 0) { $userId = $users[0].userId }
    }
    else {
        $userId = $users.userId
    }
}
if (-not $userId) {
    $reg = Invoke-Api POST "/user/register" @{
        userName     = "Tester$(Get-Random -Maximum 99999)"
        email        = "t$(Get-Random)@ex.com"
        password     = "Senha1234!"
        profilePhoto = ""
    }
    $created = $reg.Content | ConvertFrom-Json
    $userId = $created.userId
    if (-not $userId) { $userId = $created.UserId }
}
Write-Host "`n>>> Usando UserId = $userId" -ForegroundColor Magenta

# 4) Categoria
$catResp = Invoke-Api POST "/api/Category/registerCategory" @{
    categoryName        = "Cat PS1 $(Get-Date -Format 'HHmmss')"
    categoryDescription = "via TestApiCrud"
    type                = 2
}
$catJson = $catResp.Content | ConvertFrom-Json
$categoryId = $catJson.categoryId
if (-not $categoryId) { $categoryId = $catJson.CategoryId }
Write-Host "`n>>> Usando CategoryId = $categoryId" -ForegroundColor Magenta

# 5) Transação — criar
$txBody = @{
    transactionDescription = "Lançamento teste PS1"
    transactionValue       = 12.34
    date                   = (Get-Date).ToUniversalTime().ToString("o")
    transactionTypeId      = 2
    categoryId             = $categoryId
    accountId              = 1
    userId                 = $userId
    status                 = 2
}
$txCreate = Invoke-Api POST "/api/Transaction" $txBody
$tx = $txCreate.Content | ConvertFrom-Json
$txId = $tx.transactionId
if (-not $txId) { $txId = $tx.TransactionId }
Write-Host "`n>>> Transação criada id = $txId" -ForegroundColor Magenta

# 6) Listar transações
Invoke-Api GET "/api/Transaction?userId=$userId" | Out-Null

# 7) Atualizar
Invoke-Api PUT "/api/Transaction/$txId" @{
    transactionId          = [int]$txId
    transactionDescription = "Lançamento teste PS1 (editado)"
    transactionValue       = 15.00
    date                   = (Get-Date).ToUniversalTime().ToString("o")
    transactionTypeId      = 2
    categoryId             = $categoryId
    accountId              = 1
    status                 = 2
} | Out-Null

# 8) Excluir
Invoke-Api DELETE "/api/Transaction/$txId" | Out-Null

# 9) Remover conta extra (se criada)
if ($newAccountId -and $newAccountId -ne 1) {
    try {
        Invoke-Api DELETE "/api/Account/$newAccountId" | Out-Null
    }
    catch {
        Write-Host "(Conta $newAccountId não removida — pode ter FK ou ser uso)`n" -ForegroundColor DarkYellow
    }
}

Write-Host "`n=== Concluído sem erros fatais ===" -ForegroundColor Green
