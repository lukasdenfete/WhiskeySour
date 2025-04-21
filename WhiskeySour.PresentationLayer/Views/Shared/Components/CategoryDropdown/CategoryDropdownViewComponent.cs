using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;

namespace WhiskeySour.Views.Shared.Components;

public class CategoryDropdownViewComponent : ViewComponent
{
    private readonly AppDbContext _dbContext;

    public CategoryDropdownViewComponent(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _dbContext.Categories.ToListAsync();
        return View("_CategoryDropdownPartial", categories);
    }
}