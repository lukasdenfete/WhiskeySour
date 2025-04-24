using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Controllers;

public class ForumController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ForumController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var threads = _context.Threads
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.Created)
            .ToListAsync();
        
        return View(threads);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Create(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError("", "Both title and content are required.");
            return View();
        }
        var user = await _userManager.GetUserAsync(User);
        var thread = new DataLayer.Thread
        {
            Title = title,
            Content = content,
            Created = DateTime.Now,
            CreatedById = user.Id,
        };
        _context.Threads.Add(thread);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var thread = await _context.Threads
            .Include(t => t.CreatedBy)
            .Include(t => t.Comments)
            .ThenInclude(c => c.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (thread == null)
        {
            return NotFound();
        }
        return View(thread);
    }
    
    
}