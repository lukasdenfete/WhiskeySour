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
        
        var product = await _context.Products.FindAsync(productId);
        if(product == null) return NotFound();
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        var quantityInCart = existingItem?.Quantity ?? 0;
        var newQuantity = quantity + quantityInCart;
        //för att redirecta tillbaka dit man kom från
        string refererUrl = Request.Headers["Referer"].ToString();

        if (newQuantity > product.Quantity)
        {
            TempData["Error"] = $"No more {product.Name} left in stock.";
            //Redirect tillbaka dit man kom från
            return Redirect(refererUrl);
        }
        
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
        TempData["Success"] = $"{product.Name} added to cart!";
        return Redirect(refererUrl);
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
        var item = await _context.CartItems.Include
            (i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        
        if (item != null)
        {
            var newQuantity = item.Quantity + change;
            if (change > 0)
            {
                //Kolla om nya antalet överskrider lagersaldot
                if (newQuantity <= item.Product.Quantity)
                {
                    item.Quantity = newQuantity;
                }
                else
                {
                    TempData["Error"] = "No more products in stock.";
                }
            }
            else
            {
                item.Quantity = newQuantity;
            }

            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}