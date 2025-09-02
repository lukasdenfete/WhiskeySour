using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class CommentViewModel
{
    [Required]
    public string Content { get; set; }
    public int CommentId { get; set; }
    public string CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime Created { get; set; }
    public byte[]? Image { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsEdited => EditedAt.HasValue;
    public int Likes { get; set; }
    public bool HasLiked { get; set; }
}