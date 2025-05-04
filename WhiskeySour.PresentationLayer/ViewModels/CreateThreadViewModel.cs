using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class CreateThreadViewModel
{
    [Required(ErrorMessage = "Thread Title is required")]
    public string ThreadTitle { get; set; }

    [Required(ErrorMessage = "Thread content is required")]
    public string ThreadContent { get; set; }
}