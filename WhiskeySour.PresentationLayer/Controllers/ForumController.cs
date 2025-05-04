using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

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
        var threads = await _context.Threads
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.Created)
            .ToListAsync();

        var fvm = threads.Select(t => new ForumViewModel
        {
            ThreadId = t.Id,
            ThreadTitle = t.Title,
            ThreadContent = t.Content,
            Created = t.Created,
            CreatedByName = t.CreatedBy.FirstName + " " + t.CreatedBy.LastName,
            Comments = new List<CommentViewModel>() // tom för Index
        }).ToList();

        return View(fvm); 
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> Create(CreateThreadViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var thread = new DataLayer.Thread
            {
                Title = model.ThreadTitle,
                Content = model.ThreadContent,
                Created = DateTime.Now,
                CreatedById = user.Id,
            };

            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        return View(model);
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

        var fvm = new ForumViewModel
        {
            ThreadId = thread.Id,
            ThreadTitle = thread.Title,
            ThreadContent = thread.Content,
            CreatedByName = thread.CreatedBy.FirstName + " " + thread.CreatedBy.LastName,
            Created = thread.Created,
            Comments = thread.Comments.Select(c => new CommentViewModel
            {
                Content = c.Content,
                CreatedByName = c.CreatedBy.FirstName + " " + c.CreatedBy.LastName,
                Created = c.Created
            }).ToList()
        };
        return View(fvm);
    }
    
    
}