using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Web.Helpers;

namespace FinanceControl.Web.Models.ViewModels.Profile;

public sealed class ProfileViewModel
{
    public UserUpdateDto Input { get; set; } = new();

    public IFormFile? ProfilePhotoFile { get; set; }

    public bool TemUsuario { get; set; }

    public string? ErroPagina { get; set; }

    public string? SucessoMensagem { get; set; }

    public string AvatarSrc => UserAvatarHelper.BuildAvatarSrc(Input.ProfilePhoto, Input.UserName);
}
