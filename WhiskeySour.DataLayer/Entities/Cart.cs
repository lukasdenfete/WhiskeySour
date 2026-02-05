using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiskeySour.DataLayer;

public class Cart
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    [NotMapped]
    public decimal TotalPrice => Items?.Sum(x => x.Quantity * x.Product.Price) ?? 0;
}