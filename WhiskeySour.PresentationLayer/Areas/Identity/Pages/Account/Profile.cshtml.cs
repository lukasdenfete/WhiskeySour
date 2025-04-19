using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Areas.Identity.Pages.Account;

public class ProfileModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public ProfileModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        Email = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
    }
    

}