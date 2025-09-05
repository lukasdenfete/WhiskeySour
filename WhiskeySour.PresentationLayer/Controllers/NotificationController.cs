using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Controllers;

public class NotificationController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public NotificationController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
       
    }
}