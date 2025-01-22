using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Domain;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool isInStock => Quantity > 0;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Order>? OrdersNavigation { get; set; } = new List<Order>();
}