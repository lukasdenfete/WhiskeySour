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
                FromUserName = n.FromUser != null ? n.FromUser.FirstName + " " + n.FromUser.LastName : null, 
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
}