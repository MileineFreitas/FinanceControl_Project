using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Web.Models.ViewModels.Profile;

public sealed class ProfileViewModel
{
    public UserUpdateDto Input { get; set; } = new();

    public IFormFile? ProfilePhotoFile { get; set; }

    public bool TemUsuario { get; set; }

    public string? ErroPagina { get; set; }

    public string? SucessoMensagem { get; set; }

    public string AvatarSrc =>
        !string.IsNullOrWhiteSpace(Input.ProfilePhoto)
            ? (Input.ProfilePhoto.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                ? Input.ProfilePhoto
                : $"data:image/jpeg;base64,{Input.ProfilePhoto}")
            : $"https://api.dicebear.com/7.x/avataaars/svg?seed={Uri.EscapeDataString(Input.UserName ?? "finance")}";
}
