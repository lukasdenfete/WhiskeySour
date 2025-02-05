using WhiskeySour.Domain;

namespace WhiskeySour.Web.ViewModels;

public class SearchViewModel
{
    public string Query { get; set; }
    public List<Product> Products { get; set; }
    
}