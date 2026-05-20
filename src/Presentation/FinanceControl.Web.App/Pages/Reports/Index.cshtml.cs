using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.Reports;

public class IndexModel : PageModel
{
    public IActionResult OnGet() =>
        Redirect("/dashboards/geral");
}
