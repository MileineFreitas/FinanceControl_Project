using FinanceControl.Domain.Entities;

namespace FinanceControl.Web.Models.ViewModels.Register;

public sealed class RegisterViewModel
{
    public RegisterUserDto Register { get; set; } = new();

    public IFormFile? ProfilePhotoFile { get; set; }

    public string? ErrorMessage { get; set; }
}
