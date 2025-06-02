using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class CommentViewModel
{
    [Required]
    public string Content { get; set; }
    public string CreatedByName { get; set; }
    public DateTime Created { get; set; }
    public byte[]? Image { get; set; }
}