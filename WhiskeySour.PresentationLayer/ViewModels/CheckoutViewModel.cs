using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WhiskeySour.DataLayer;
namespace WhiskeySour.Web.ViewModels;

public class CheckoutViewModel
{
    [ValidateNever]
    public Cart? Cart { get; set; }
    
    [Required(ErrorMessage = "First name is  required")]
    [Display(Name = "First name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is  required")]
    [Display(Name = "Last name")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Please select a country")]
    public string Country { get; set; }

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; } = "credit"; // Default
    
    [Display(Name = "Credit card number")]
    [RegularExpression(@"^[0-9]{16}$", ErrorMessage = "Card number must be exactly 16 digits")]
    public string? CreditCardNumber { get; set; }

    public string? CreditCardName { get; set; }
    
    [Display(Name = "Expiration")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Format must be MM/YY (e.g. 05/25)")]
    public string? CreditCardExpiration { get; set; }
        
    [Display(Name = "CVV")]
    [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "CVV must be 3 digits")]
    public string? CreditCardCvv { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number")]
    [RegularExpression(@"^(\+46|0)7[0-9]{8}$", ErrorMessage = "Must be a valid swedish mobile number")]
    public string? SwishNumber { get; set; }
}