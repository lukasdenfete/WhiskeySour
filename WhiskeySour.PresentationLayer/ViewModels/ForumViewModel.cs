namespace WhiskeySour.Web.ViewModels;

public class ForumViewModel
{
    public int ThreadId { get; set; }
    public string ThreadTitle { get; set; }
    public string ThreadContent { get; set; }
    public string CreatedByName { get; set; }
    public DateTime Created { get; set; }
    public List<CommentViewModel> Comments { get; set; }
}