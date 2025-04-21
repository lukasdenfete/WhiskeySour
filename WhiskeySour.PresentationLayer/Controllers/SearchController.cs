using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class SearchController : Controller
{
    private readonly AppDbContext _context;

    public SearchController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Search(string searchString)
    {
        var vm = new SearchViewModel
        {
            Query = searchString,
            Products = string.IsNullOrWhiteSpace(searchString)
                ? new List<Product>()
                : _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Name.Contains(searchString) || p.Category.Name.Contains(searchString))
                    .ToList()
        };
        return View("Search", vm);
    }
    
}