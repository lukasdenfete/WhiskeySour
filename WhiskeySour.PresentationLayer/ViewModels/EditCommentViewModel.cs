using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class EditCommentViewModel
{
    public int CommentId { get; set; }

    [Required]
    public string Content { get; set; }

    public IFormFile? ImageFile { get; set; }

    public byte[]? ExistingImage { get; set; }

    public bool RemoveImage { get; set; }

    public int ThreadId { get; set; }
}