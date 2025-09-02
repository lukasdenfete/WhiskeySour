using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            CommentCount = _context.Comments.Count(c => c.ThreadId == t.Id)
        }).ToList();

        return View(fvm); 
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(CreateCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", new { id = model.ThreadId });
        }
        var user = await _userManager.GetUserAsync(User);
        var comment = new Comment
        {
            ThreadId = model.ThreadId,
            Content = model.Content,
            Created = DateTime.Now,
            CreatedById = user.Id
        };
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await model.ImageFile.CopyToAsync(ms);
            comment.Image = ms.ToArray();
        }
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = model.ThreadId });
    }

    public IActionResult Create()
    {
        var model = new CreateThreadViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> Create(CreateThreadViewModel model) //skapa tråd
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
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.ImageFile.CopyToAsync(ms);
                thread.Image = ms.ToArray();
            }

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
            CreatedById = thread.CreatedBy.Id,
            CreatedByName = thread.CreatedBy.FirstName + " " + thread.CreatedBy.LastName,
            CommentCount = thread.Comments.Count,
            Created = thread.Created,
            ThreadImage = thread.Image,
            EditedAt = thread.EditedAt,
            Comments = thread.Comments.Select(c => new CommentViewModel
            {
                Content = c.Content,
                CreatedByName = c.CreatedBy.FirstName + " " + c.CreatedBy.LastName,
                CreatedById = c.CreatedBy.Id,
                CommentId = c.Id,
                Created = c.Created,
                Image = c.Image,
                EditedAt = c.EditedAt
            }).ToList(),
            NewComment = new CreateCommentViewModel
            {
                ThreadId = thread.Id,
            }
        };
        return View(fvm);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteThread(int id)
    {
        var thread = await _context.Threads
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (thread.CreatedById == user.Id || isAdmin)
        {
            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            return Forbid();
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (comment.CreatedById == user.Id || isAdmin)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = comment.ThreadId });
        }
        else
        {
            return Forbid();
        }
        
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, string type)
    {
        var user  = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (type == "thread")
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == id);
            if (thread == null) return NotFound();
            if (thread.CreatedById != user.Id && !isAdmin) return Forbid();
            thread.Image = null;
        } else if (type == "comment")
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return NotFound();

            if (comment.CreatedById != user.Id && !isAdmin) return Forbid();

            comment.Image = null;
        } else
        {
            return BadRequest("Unknown type.");
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = (type == "thread" ? id : _context.Comments
            .FirstOrDefault(c => c.Id == id)!.ThreadId) });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var thread = await _context.Threads
            .FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (thread.CreatedById != user.Id && !isAdmin)
        {
            return Forbid();
        }

        var model = new EditThreadViewModel
        {
            ThreadId = thread.Id,
            ThreadTitle = thread.Title,
            ThreadContent = thread.Content,
            ExistingImage = thread.Image
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(EditThreadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == model.ThreadId);
        if (thread == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (thread.CreatedById != user.Id && !isAdmin)
        {
            return Forbid();
        }
        thread.Title = model.ThreadTitle;
        thread.Content = model.ThreadContent;
        thread.EditedAt = DateTime.Now;
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await model.ImageFile.CopyToAsync(ms);
            thread.Image = ms.ToArray();
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = thread.Id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditComment(int id)
    {
        var comment = await _context.Comments
                .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (comment.CreatedById != user.Id && !isAdmin)
        {
            return Forbid();
        }
        var model = new EditCommentViewModel
        {
            CommentId = comment.Id,
            Content = comment.Content,
            ExistingImage = comment.Image,
            ThreadId = comment.ThreadId
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(EditCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == model.CommentId);
        if (comment == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (comment.CreatedById != user.Id && !isAdmin)
        {
            return Forbid();
        }
        comment.Content = model.Content;

        if (model.RemoveImage)
        {
            comment.Image = null;
        }
        else if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await model.ImageFile.CopyToAsync(ms);
            comment.Image = ms.ToArray();
        }
        comment.EditedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = comment.ThreadId });
    }
    
}