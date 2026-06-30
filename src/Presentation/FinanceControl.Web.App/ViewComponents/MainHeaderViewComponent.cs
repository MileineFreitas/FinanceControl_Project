using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.ViewComponents;

public sealed class MainHeaderViewComponent(IUserCliService userCli) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new MainHeaderViewModel();
        var userId = ViewContext.HttpContext.User.GetUserId();

        if (userId == null)
        {
            model.UserName = ViewContext.HttpContext.User.GetUserDisplayName();
            return View("~/Views/Shared/_MainHeader.cshtml", model);
        }

        try
        {
            var user = await userCli.GetByIdAsync(userId.Value);
            if (user != null)
            {
                model.UserName = user.UserName;
                model.AvatarSrc = UserAvatarHelper.BuildAvatarSrc(user.ProfilePhoto, user.UserName);
            }
            else
            {
                model.UserName = ViewContext.HttpContext.User.GetUserDisplayName();
            }
        }
        catch
        {
            model.UserName = ViewContext.HttpContext.User.GetUserDisplayName();
        }

        return View("~/Views/Shared/_MainHeader.cshtml", model);
    }
}
