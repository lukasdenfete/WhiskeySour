using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

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
        var user = await _userManager.GetUserAsync(User);
        var notifications = await _context.Notifications
            .Where(n => n.UserId == user.Id)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationViewModel
            {
                Id = n.Id,
                Type = n.Type,
                FromUserId = n.FromUserId,
                FromUserName = n.FromUser != null ? n.FromUser.FirstName + " " + n.FromUser.LastName : null, //username är mailadress
                ThreadTitle = n.Thread != null ? n.Thread.Title : null,
                ThreadId = n.ThreadId,
                CommentText = n.Comment != null ? n.Comment.Content : null, //Null-kontroller på navigation properties
                CommentId = n.CommentId,
                MessageText = n.Message != null ? n.Message.Content : null,
                MessageId = n.MessageId,
                IsRead = n.isRead,
                CreatedAt = n.CreatedAt,
            })
            .ToListAsync();
        
        return View(notifications);
        
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var notification = await _context.Notifications
            .Where(n => n.Id == id && n.UserId == user.Id)
            .FirstOrDefaultAsync();
        if (notification == null)
        {
            return NotFound();
        }
        notification.isRead = true;
        await _context.SaveChangesAsync();

         switch (notification.Type)
            {
                case NotificationType.NewComment:
                case NotificationType.NewThreadFromFollowee:
                    return RedirectToAction("Details", "Forum", new { id = notification.ThreadId });

                case NotificationType.NewMessage:
                    return RedirectToAction("Conversation", "Message", new { id = notification.FromUserId });

                case NotificationType.NewFollower:
                    return RedirectToAction("Details", "Profile", new { id = notification.FromUserId });

                default:
                    return RedirectToAction("Index", "Notification");
            }
                
        }
    }
