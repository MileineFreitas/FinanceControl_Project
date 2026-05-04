using FinanceControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages;

public class HomeModel : PageModel
{
    public IReadOnlyList<SpendingRow> SpendingItems { get; private set; } = Array.Empty<SpendingRow>();

    public IReadOnlyList<TransactionRow> Transactions { get; private set; } = Array.Empty<TransactionRow>();

    public void OnGet()
    {
        SpendingItems =
        [
            new SpendingRow("Moradia", "R$ 1.687,00", "hs-legend-bar--primary"),
            new SpendingRow("Investimentos", "R$ 1.200,00", "hs-legend-bar--tertiary"),
            new SpendingRow("Estilo de vida", "R$ 933,00", "hs-legend-bar--secondary"),
            new SpendingRow("Outros", "R$ 1.000,00", "hs-legend-bar--muted"),
        ];

        Transactions =
        [
            new TransactionRow(
                "Apple Store Soho", "Eletrônicos", "12 out., 10:45", "shopping_bag",
                "-R$ 1.299,00", "Saídas", "hs-trans--primary", "hs-trans-icon--primary", ""),
            new TransactionRow(
                "Repasse mensal Stripe", "Salário", "10 out., 09:00", "payments",
                "+R$ 8.400,00", "Entradas", "hs-trans--tertiary", "hs-trans-icon--tertiary", "hs-trans-amt--in"),
            new TransactionRow(
                "The Green Bistro", "Alimentação", "09 out., 20:30", "restaurant",
                "-R$ 84,50", "Saídas", "hs-trans--primary", "hs-trans-icon--primary", ""),
            new TransactionRow(
                "Airbnb — Tóquio", "Viagem", "07 out., 14:15", "travel_explore",
                "-R$ 420,00", "Saídas", "hs-trans--primary", "hs-trans-icon--primary", ""),
        ];
    }
}
