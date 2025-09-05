using WhiskeySour.DataLayer;

namespace WhiskeySour.Web.ViewModels;

public class NotificationViewModel
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    public string FromUserId { get; set; }
    public string FromUserName { get; set; }
    public string? ThreadTitle { get; set; }
    public int? ThreadId { get; set; }
    public string? CommentText { get; set; }
    public int? CommentId { get; set; }
    public string? MessageText { get; set; }
    public int? MessageId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}