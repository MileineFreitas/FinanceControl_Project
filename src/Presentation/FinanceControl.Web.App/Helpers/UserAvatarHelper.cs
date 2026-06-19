namespace FinanceControl.Web.Helpers;

public static class UserAvatarHelper
{
    public static string BuildAvatarSrc(string? profilePhoto, string? userName)
    {
        if (!string.IsNullOrWhiteSpace(profilePhoto))
        {
            return profilePhoto.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                ? profilePhoto
                : $"data:image/jpeg;base64,{profilePhoto}";
        }

        return $"https://api.dicebear.com/7.x/avataaars/svg?seed={Uri.EscapeDataString(userName ?? "finance")}";
    }
}
