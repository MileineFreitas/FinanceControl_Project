using FinanceControl.Domain.Entities;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages;

public class RegisterModel : PageModel
{
    private readonly IFinanceControlApiClient _api;

    public RegisterModel(IFinanceControlApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public RegisterUserDto Register { get; set; } = new();

    public IFormFile? ProfilePhotoFile { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!TryValidateModel(Register))
            return Page();

        if (ProfilePhotoFile is { Length: > 0 })
        {
            await using var ms = new MemoryStream();
            await ProfilePhotoFile.CopyToAsync(ms, cancellationToken);
            Register.ProfilePhoto = Convert.ToBase64String(ms.ToArray());
        }

        try
        {
            var response = await _api.RegisterAsync(Register, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Erro ao criar conta: {await response.Content.ReadAsStringAsync(cancellationToken)}";
                return Page();
            }

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
