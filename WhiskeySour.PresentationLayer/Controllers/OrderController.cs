using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;


namespace WhiskeySour.Controllers;

public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    // Glöm inte: using WhiskeySour.Web.ViewModels;

[HttpPost]
public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var cart = await _context.Carts
        .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    model.Cart = cart;

    if (cart == null || !cart.Items.Any())
    {
        return RedirectToAction("Index", "Cart");
    }
    if (!ModelState.IsValid)
    {
        return View("../Cart/Checkout", model);
    }
    var order = new Order
    {
        UserId = userId,
        OrderDate = DateTime.Now,
        TotalPrice = cart.TotalPrice,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Address = model.Address,
        City = model.City,
        Country = model.Country
    };
    
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    foreach (var item in cart.Items)
    {
        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Product.Price
        };
        _context.OrderItems.Add(orderItem);
        
        var product = item.Product;
        product.Quantity -= item.Quantity;
    }
    
    _context.CartItems.RemoveRange(cart.Items);
    await _context.SaveChangesAsync();

    return RedirectToAction("OrderConfirmation", new {id = order.Id});
}

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        var orderEntity = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (orderEntity == null) return NotFound();

        var viewModel = new OrderViewModel
        {
            OrderId = orderEntity.Id,
            OrderDate = orderEntity.OrderDate.ToString("yyyy-MM-dd HH:mm"),
            TotalPrice = orderEntity.TotalPrice,
            CustomerName = $"{orderEntity.FirstName} {orderEntity.LastName}",
            Address = orderEntity.Address,
            City = orderEntity.City,
            Items = orderEntity.OrderItems.Select(item => new OrderItemViewModel
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price,
            }).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                TotalPrice = o.TotalPrice,
                Status = "Completed", // hårdkodat tills man lägger in status i DB

                Items = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();

        return View(orders);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orderEntity = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (orderEntity == null) return NotFound();

        var viewModel = new OrderViewModel
        {
            OrderId = orderEntity.Id,
            OrderDate = orderEntity.OrderDate.ToString("yyyy-MM-dd HH:mm"),
            TotalPrice = orderEntity.TotalPrice,
            CustomerName = $"{orderEntity.FirstName} {orderEntity.LastName}",
            Address = orderEntity.Address,
            City = orderEntity.City,
            Items = orderEntity.OrderItems.Select(item => new OrderItemViewModel
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Price,
            }).ToList()
        };
        return View(viewModel); 
    }
    
}