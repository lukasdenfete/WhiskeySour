
namespace WhiskeySour.DataLayer;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}