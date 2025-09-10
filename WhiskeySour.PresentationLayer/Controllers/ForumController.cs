using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;
using Thread = WhiskeySour.DataLayer.Thread;

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
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == model.ThreadId);
        
        if (thread.CreatedById != user.Id) { //inga notiser när man kommenterar på sin egen tråd
            var notification = new Notification
            {
                UserId = thread.CreatedById,
                FromUserId = user.Id,
                Type = NotificationType.NewComment,
                CommentId = comment.Id,
                Comment = comment,
                isRead = false,
                CreatedAt = DateTime.Now,
                ThreadId = thread.Id

            };
            _context.Notifications.Add(notification);
        }
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
            var thread = new Thread
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
            var followers = _context.Follows.Where(f => f.FolloweeId == user.Id)
                .Select(f => f.Follower)
                .ToList();
            foreach (var follower in followers)
            {
                var notification = new Notification
                {
                    UserId = follower.Id,
                    FromUserId = user.Id,
                    Type = NotificationType.NewThreadFromFollowee,
                    ThreadId = thread.Id,
                    Thread = thread,
                    isRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(notification);
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
                EditedAt = c.EditedAt,
                Likes = _context.CommentLikes.Count(cl => cl.CommentId == c.Id),
                HasLiked = User.Identity.IsAuthenticated
                            && _context.CommentLikes.Any(cl => cl.CommentId == c.Id && cl.UserId == _userManager.GetUserId(User)),
                ProfilePicture = c.CreatedBy.ProfilePicture
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
            .Include(t => t.Comments)
                .ThenInclude(c => c.CommentLikes)
            .Include(t => t.Comments)
                .ThenInclude(c => c.Notifications)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("Admin");
        if (thread.CreatedById == user.Id || isAdmin)
        {
            // Tar bort trådnotiser manuellt pga fk constraints och multiple cascade paths
            var threadNotifications = _context.Notifications.Where(n => n.ThreadId == thread.Id);
            _context.Notifications.RemoveRange(threadNotifications);

            // Tar bort kommentarnotiser manuellt pga fk constraints och multiple cascade paths
            foreach (var comment in thread.Comments)
            {
                var commentNotifications = _context.Notifications.Where(n => n.CommentId == comment.Id);
                _context.Notifications.RemoveRange(commentNotifications);
            }

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
            .Include(c => c.CommentLikes)
            .Include(c => c.Notifications)
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

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LikeComment(int commentId)
    {
        var user = await _userManager.GetUserAsync(User);
        var existingLike = await _context.CommentLikes
            .FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == user.Id);

        if (existingLike == null)
        {
            _context.CommentLikes.Add(new CommentLike
            {
                CommentId = commentId,
                UserId = user.Id
            });
        }
        else
        {
            _context.CommentLikes.Remove(existingLike);
        }
        await _context.SaveChangesAsync();

        var threadId = (await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId))!.ThreadId; // redirecta till rätt tråd
        return RedirectToAction("Details", new { id = threadId });
    }
}