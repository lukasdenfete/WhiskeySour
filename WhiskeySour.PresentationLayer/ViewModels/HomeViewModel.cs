using WhiskeySour.DataLayer;
using Thread = System.Threading.Thread; // Eller ditt namespace för entiteter

namespace WhiskeySour.Web.ViewModels;

public class HomeViewModel
{
    public List<Product> LatestProducts { get; set; } = new List<Product>();
    public List<WhiskeySour.DataLayer.Thread> LatestThreads { get; set; } = new List<WhiskeySour.DataLayer.Thread>();
    public List<User> NewUsers { get; set; } = new List<User>();
    // Inlägg från de man följer (Om inloggad)
    public List<WhiskeySour.DataLayer.Thread> Feed { get; set; } = new List<WhiskeySour.DataLayer.Thread>();
    public bool IsLoggedIn { get; set; }
}