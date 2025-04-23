using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;

    public ProfileController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var vm = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.UserName,
            ProfilePicture = user.ProfilePicture,
        };
        return View(vm);
    }

    [Authorize(Roles = "User, Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        var vm = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.UserName,
            ProfilePicture = user.ProfilePicture
        };
        return View(vm);
    }

    [Authorize(Roles = "User, Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        var user = await _userManager.GetUserAsync(User);
        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
        user.Email = vm.Email;
        if (vm.ImageFile != null && vm.ImageFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await vm.ImageFile.CopyToAsync(ms);
            user.ProfilePicture = ms.ToArray();
        }
        
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Profile");
        }
        return View(vm);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var user = await _userManager.FindByIdAsync(id);
        if (id == currentUser.Id)
        {
            return RedirectToAction("Index", "Profile");
        }
        var pvm = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePicture
        };
        return View(pvm);
    }
}