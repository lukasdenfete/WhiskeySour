using Microsoft.AspNetCore.Mvc.Rendering;
using WhiskeySour.Domain;

namespace WhiskeySour.Web.ViewModels;

public class ProductViewModel
{
    public Product Product { get; set; }
    public IEnumerable<Category>? Categories { get; set; }
    
    public IEnumerable<Product>? ProductsNavigation { get; set; }
    public IFormFile? ImageFile { get; set; }

}