using Microsoft.AspNetCore.Identity;
namespace WhiskeySour.DataLayer;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public ICollection<Order> OrdersNavigation { get; set; } = new List<Order>();

    
}