using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.Web.ViewModels;

public class ForumViewModel
{
    public int ThreadId { get; set; }
    [Required(ErrorMessage = "Title is required")]
    public string ThreadTitle { get; set; }
    [Required(ErrorMessage = "Content is required")]
    public string ThreadContent { get; set; }
    public string CreatedByName { get; set; }
    public DateTime Created { get; set; }
    public List<CommentViewModel> Comments { get; set; }
    public CreateCommentViewModel NewComment { get; set; }
}