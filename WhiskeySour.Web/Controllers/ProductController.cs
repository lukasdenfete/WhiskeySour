using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        //hämtar produkter och inkluderar kategorin
        var products = _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductViewModel
            {
                Product = p,
                Categories = _context.Categories.ToList()
            }).ToList();
            
            
        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        //hämtar kategorier från db och skickar till vyn via viewmodel
        var categories = _context.Categories.ToList();
        var pvm = new ProductViewModel
        {
            Categories = categories
        };
        return View(pvm);
    }

    [HttpPost]
    public IActionResult Create(ProductViewModel productViewModel)
    {
        //om modelstate inte är valid visas formuläret igen med befintlig data
        if (!ModelState.IsValid)
        {
            var categories = _context.Categories.ToList();
            productViewModel.Categories = categories;
            return View(productViewModel);
        }
        
        //skapar produkt från viewmodeldata
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

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _context.Products.
            Include(p => p.Category).
            FirstOrDefault(p => p.ProductId == id);
        var vm = new ProductViewModel
        {
            Product = product,
            Categories = _context.Categories.ToList()
        };
        return View(vm);

    }

    [HttpPost]
    public IActionResult Edit(int id, ProductViewModel pvm)
    {
        if (!ModelState.IsValid)
        {
            pvm.Categories = _context.Categories.ToList();
            return View(pvm);
        }
        // hämta nuvarande produkt från db
        var currentProduct = _context.Products.FirstOrDefault(p => p.ProductId == id);
        
        //uppdatera produkten
        currentProduct.Name = pvm.Product.Name;
        currentProduct.Description = pvm.Product.Description;
        currentProduct.Price = pvm.Product.Price;
        currentProduct.Quantity = pvm.Product.Quantity;
        currentProduct.CategoryId = pvm.Product.CategoryId;
        
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = currentProduct.ProductId });
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == id);

        var vm = new ProductViewModel
        {
            Product = product,
        };
        return View(vm);
    }
}
    
