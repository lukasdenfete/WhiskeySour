using WhiskeySour.DataLayer;

namespace WhiskeySour.Web.ViewModels;

public class SearchViewModel
{
    public string Query { get; set; }
    public List<Product> Products { get; set; }
    public int? SelectedCategoryId { get; set; }
    public List<Category> Categories { get; set; }
    public List<ProfileViewModel> Users { get; set; }
    
}