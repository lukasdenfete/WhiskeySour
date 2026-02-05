using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context =  context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        viewModel.IsLoggedIn = User.Identity.IsAuthenticated;
        
        viewModel.LatestProducts = await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.ProductId)
            .Take(3)
            .ToListAsync();

        viewModel.LatestThreads = await _context.Threads
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.Created)
            .Take(3)
            .ToListAsync();
        
        viewModel.NewUsers = await _context.Users
            .OrderByDescending(u => u.JoinedDate)
            .Take(5)
            .ToListAsync(); 
        
      if (viewModel.IsLoggedIn && userId != null)
      {
          // Hämta trådar skapade av personer jag följer
          viewModel.Feed = await _context.Threads
              .Include(t => t.CreatedBy)
              .Where(t => _context.Follows
                  .Where(f => f.FollowerId == userId)
                  .Select(f => f.FolloweeId)
                  .Contains(t.CreatedById))
              .OrderByDescending(t => t.Created)
              .Take(5)
              .ToListAsync();
      }
        return View(viewModel);
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