using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Views.Shared.Components.UnreadNotifications;

public class UnreadNotificationsViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public UnreadNotificationsViewComponent(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return View(0);
        }
        var count = _context.Notifications.Where(x => x.UserId == user.Id && !x.isRead).Count();
        return View("_UnreadNotificationsPartial", count);
        
    }
    
}