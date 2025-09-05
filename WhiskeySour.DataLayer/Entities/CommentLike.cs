namespace WhiskeySour.DataLayer;

public class CommentLike
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public Comment Comment { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    
}