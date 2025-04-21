using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
        if (productViewModel.ImageFile != null && productViewModel.ImageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                productViewModel.ImageFile.CopyTo(memoryStream);
                product.Image = memoryStream.ToArray();
            }
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, ProductViewModel pvm, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            pvm.Categories = _context.Categories.ToList();
            return View(pvm);
        }
        // hämta nuvarande produkt från db
        var currentProduct = await _context.Products.FindAsync(id);
        if (imageFile != null && imageFile.Length > 0)
        {
            //spara bilden som en byte array
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                currentProduct.Image = memoryStream.ToArray();
            }
             
        }
        
        //uppdatera produkten
        currentProduct.Name = pvm.Product.Name;
        currentProduct.Description = pvm.Product.Description;
        currentProduct.Price = pvm.Product.Price;
        currentProduct.Quantity = pvm.Product.Quantity;
        currentProduct.CategoryId = pvm.Product.CategoryId;
        
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = currentProduct.ProductId });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == id);
        var vm = new ProductViewModel
        {
            Product = product
        };
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id, ProductViewModel pvm)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == id);

        var relatedProducts = _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
            .ToList();

        var vm = new ProductViewModel
        {
            Product = product,
            ProductsNavigation = relatedProducts
        };
        return View(vm);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProductImage(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return View(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProductImage(int id, IFormFile image)
    {
        var product = await _context.Products.FindAsync(id);
        if (image != null && image.Length > 0)
        {
            using (var fileStream = image.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                fileStream.CopyTo(ms);
                product.Image = ms.ToArray();
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Edit", new { id = product.ProductId });
    }
}
    
