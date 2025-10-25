using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, AppDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = _userManager.GetUserId(User);
        var recentThreads = await _context.Threads.OrderByDescending(t => t.Created).Take(3).ToListAsync();
        var popularThreads = await _context.Threads.OrderByDescending(t => t.Comments.Count).Take(3).ToListAsync();
        //var followeeIds = await _context.Follows.Where(f => f.FollowerId == currentUserId)
          //  .Select(f => f.FollowerId).ToListAsync();
          var hvm = new HomeViewModel
          {
              RecentThreads = recentThreads,
              PopularThreads = popularThreads,
              
          };
          return View(hvm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}