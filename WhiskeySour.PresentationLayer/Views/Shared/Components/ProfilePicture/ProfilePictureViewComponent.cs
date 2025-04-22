using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Views.Shared.Components.ProfilePicture;

public class ProfilePictureViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;

    public ProfilePictureViewComponent(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var pvm = new ProfileViewModel()
        {
            ProfilePicture = user?.ProfilePicture
        };
        return View("_ProfilePicturePartial", pvm);
    }
}