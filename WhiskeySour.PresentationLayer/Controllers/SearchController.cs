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

    public async Task<IActionResult> Search(string searchString, int? selectedCategoryId)
    {
        var categories = await _context.Categories.ToListAsync();
        var productsQuery = _context.Products
            .Include(p => p.Category)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
        }

        if (selectedCategoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == selectedCategoryId.Value);
        }
        var products = await productsQuery.ToListAsync();
        var users = new List<User>();
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            users = await _context.Users
                .Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString))
                .ToListAsync();
        }

        var userViewModels = users.Select(u => new ProfileViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            ProfilePicture = u.ProfilePicture
        }).ToList();
            
        var vm = new SearchViewModel
        {
            Query = searchString,
            SelectedCategoryId = selectedCategoryId,
            Categories = categories,
            Products = products,
            Users = userViewModels
        };
        return View("Search", vm);
    }
    
}