
using Microsoft.AspNetCore.Identity;

namespace WhiskeySour.DataLayer;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ICollection<User> UsersNavigation { get; set; }
    public ICollection<Product> ProductsNavigation { get; set; }
}