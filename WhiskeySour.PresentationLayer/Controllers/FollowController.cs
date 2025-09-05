using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Controllers;

public class FollowController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public FollowController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Follow(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user.Id == userId) return BadRequest("You cannot follow yourself");
        
        var alreadyFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == user.Id && f.FolloweeId == userId);

        if (!alreadyFollowing)
        {
            _context.Follows.Add(new Follow
            {
                FollowerId = user.Id,
                FolloweeId = userId
            });
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Details", "Profile", new { id = userId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Unfollow(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == user.Id && f.FolloweeId == userId);
        if (follow != null)
        {
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Details", "Profile", new { id = userId });
    }
}