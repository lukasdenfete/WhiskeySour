using Microsoft.AspNetCore.Identity;
namespace WhiskeySour.Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Order> OrdersNavigation { get; set; } = new List<Order>();

    
}