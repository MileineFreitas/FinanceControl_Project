using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.ViewComponents;

public sealed class MainHeaderViewComponent(IUserCliService userCli) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new MainHeaderViewModel();

        try
        {
            var data = await userCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 50 });
            var user = data?.Result?.OrderBy(u => u.UserId).FirstOrDefault();

            if (user != null)
            {
                model.UserName = user.UserName;
                model.AvatarSrc = UserAvatarHelper.BuildAvatarSrc(user.ProfilePhoto, user.UserName);
            }
        }
        catch
        {
        }

        return View("~/Views/Shared/_MainHeader.cshtml", model);
    }
}
