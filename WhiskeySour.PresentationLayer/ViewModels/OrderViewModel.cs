namespace WhiskeySour.Web.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }
    public string OrderDate { get; set; } // Sträng för färdigformaterat datum
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Completed";
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
}