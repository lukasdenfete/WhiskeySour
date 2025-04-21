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

    public IActionResult Search(string searchString, int? selectedCategoryId)
    {
        var categories = _context.Categories.ToList();
        var products = _context.Products
            .Include(p => p.Category)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            products = products.Where(p => p.Name.Contains(searchString));
        }

        if (selectedCategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == selectedCategoryId.Value);
        }
        var vm = new SearchViewModel
        {
            Query = searchString,
            SelectedCategoryId = selectedCategoryId,
            Categories = categories,
            Products = products.ToList()
        };
        return View("Search", vm);
    }
    
}