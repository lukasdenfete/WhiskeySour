using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiskeySour.Domain;
using WhiskeySour.Infrastructure;
using WhiskeySour.Web.ViewModels;

namespace WhiskeySour.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        var products = _context.Products
            .Select(p => new ProductViewModel
            {
                Product = new Product
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId
                },
                Categories = categories
            })
            .ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var categories = _context.Categories.ToList();
        var pvm = new ProductViewModel
        {
            Categories = categories
        };
        Console.WriteLine("Kategorier: " + categories.Count);
        return View(pvm);
    }

    [HttpPost]
    public IActionResult Create(ProductViewModel productViewModel)
    {
        if (!ModelState.IsValid)
        {
            // Debug: Skriv ut alla ModelState-fel för att felsöka
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }
        }

        var product = new Product
        {
            Name = productViewModel.Product.Name,
            Description = productViewModel.Product.Description,
            Price = productViewModel.Product.Price,
            Quantity = productViewModel.Product.Quantity,
            CategoryId = productViewModel.Product.CategoryId
        };

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    }
    
