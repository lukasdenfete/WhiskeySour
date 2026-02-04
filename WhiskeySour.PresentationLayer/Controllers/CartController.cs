using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WhiskeySour.DataLayer;


namespace WhiskeySour.Controllers;
[Authorize]
public class CartController : Controller
{
    private readonly AppDbContext _context;
    
    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await GetOrCreateCartAsync(user);
        return View(cart);
    }
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await GetOrCreateCartAsync(userId);
        var currentUser = await _context.Users.FindAsync(userId);
        if (!cart.Items.Any())
        {
            return RedirectToAction("Index");
        }

        if (currentUser != null)
        {
            ViewBag.FirstName = currentUser.FirstName;
            ViewBag.LastName = currentUser.LastName;
        }
        return View(cart);
    }
    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await GetOrCreateCartAsync(userId);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product) // Inkludera produktinfo (namn, pris, bild)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        return cart;
    }

    [HttpPost]
    public async Task<IActionResult> ChangeQuantity(int itemId, int change)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item != null)
        {
            item.Quantity += change;
            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}