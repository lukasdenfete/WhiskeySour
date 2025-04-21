using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiskeySour.Areas.Identity.Pages.Account;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
}