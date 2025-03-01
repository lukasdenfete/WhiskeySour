using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.DataLayer;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool InStock => Quantity > 0;
    public int CategoryId { get; set; }
    public byte[]? Image { get; set; }
    public Category? Category { get; set; }
    public ICollection<Order>? OrdersNavigation { get; set; } = new List<Order>();
}