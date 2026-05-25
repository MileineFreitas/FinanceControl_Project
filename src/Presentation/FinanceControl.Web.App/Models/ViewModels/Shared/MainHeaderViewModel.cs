using FinanceControl.Web.Helpers;

namespace FinanceControl.Web.Models.ViewModels.Shared;

public sealed class MainHeaderViewModel
{
    public string? UserName { get; set; }

    public string AvatarSrc { get; set; } = UserAvatarHelper.BuildAvatarSrc(null, null);

    public string DisplayName =>
        string.IsNullOrWhiteSpace(UserName) ? "Meu Perfil" : UserName;
}
