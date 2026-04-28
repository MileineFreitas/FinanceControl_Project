using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.Transactions;

/// <summary>Mantém o alias em inglês usado no menu antigo (/transactions → /transacoes).</summary>
public class AliasModel : PageModel
{
    public IActionResult OnGet() => Redirect("/transacoes");
}
