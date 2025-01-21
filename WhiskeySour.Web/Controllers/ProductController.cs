using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.Domain;
using WhiskeySour.Infrastructure;

namespace WhiskeySour.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.ToListAsync());
    }
    
}