using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class EditThreadViewModel
{
    public int ThreadId { get; set; }
    [Required]
    public string ThreadTitle { get; set; }
    [Required]
    public string ThreadContent { get; set; }
    public IFormFile? ImageFile { get; set; }
    public byte[]? ExistingImage { get; set; }
}