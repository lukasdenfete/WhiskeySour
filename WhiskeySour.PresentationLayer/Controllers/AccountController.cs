using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiskeySour.Areas.Identity.Pages.Account;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
   /* [HttpGet]
    public IActionResult Login()
    {
        LoginModel model = new LoginModel();
        return View(model);
    }*/
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterModel rm)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> AddProfilePicture()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(User);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddProfilePicture(IFormFile image)
    {
        var user  = await _userManager.GetUserAsync(User);
        if (image != null && image.Length > 0)
        {
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            user.ProfilePicture = ms.ToArray();
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction("Profile", "Account");
    }
}