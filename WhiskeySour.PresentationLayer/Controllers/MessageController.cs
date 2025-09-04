using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Controllers;

public class MessageController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public MessageController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var messages = await _context.Messages
            .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        var conversations = messages
            .GroupBy(m => m.SenderId == user.Id ? m.ReceiverId : m.SenderId) // är inloggade användaren avsändare grupperas på receiver, är man mottagare grupperas på sender 
            .Select(g => g.OrderByDescending(m => m.SentAt).First())
            .ToList();
        
        return View(conversations);
    }

    public async Task<IActionResult> Conversation(string id) // userId för andra användaren
    {
        var user = await _userManager.GetUserAsync(User);
        var messages = await _context.Messages
            .Where(m => (m.SenderId == user.Id && m.ReceiverId == id) ||
                       (m.SenderId == id && m.ReceiverId == user.Id))
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
        ViewBag.PartnerId = id; //id:t på andra användaren i konversationen, för att kunna skicka nya meddelanden i vyn
        return View(messages);
    }

    public async Task<IActionResult> Send(string receiverId, string content)
    {
        var user = await _userManager.GetUserAsync(User);
        if (string.IsNullOrWhiteSpace(content))
        {
            return RedirectToAction("Conversation", new { id = receiverId });
        }
        var message = new Message
        {
            Content = content,
            SenderId = user.Id,
            ReceiverId = receiverId,
            SentAt = DateTime.Now
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return RedirectToAction("Conversation", new { id = receiverId });
    }
}