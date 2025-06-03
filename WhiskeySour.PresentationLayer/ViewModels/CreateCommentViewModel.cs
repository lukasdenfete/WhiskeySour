using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class CreateCommentViewModel
{
    [Required(ErrorMessage = "You can't post an empty comment.")]
    public string Content { get; set; }
    public int ThreadId { get; set; }
    public IFormFile? ImageFile { get; set; }
}