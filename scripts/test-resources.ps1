$dll = 'c:\Users\Acer\source\repos\MileineFreitas\FinanceControl_Project\src\Presentation\FinanceControl.Web.App\bin\Debug\net8.0\FinanceControl.Web.dll'
$asm = [System.Reflection.Assembly]::LoadFrom($dll)
Write-Host 'Manifest resources:'
$asm.GetManifestResourceNames() | ForEach-Object { Write-Host "  $_" }
$rm = New-Object System.Resources.ResourceManager('FinanceControl.Web.Resources.SharedResources', $asm)
Write-Host "Settings.PageTitle = $($rm.GetString('Settings.PageTitle'))"
Write-Host "Nav.Dashboard = $($rm.GetString('Nav.Dashboard'))"
