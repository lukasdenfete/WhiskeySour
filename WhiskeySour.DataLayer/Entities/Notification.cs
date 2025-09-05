using Microsoft.Data.SqlClient;

namespace WhiskeySour.DataLayer;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public NotificationType Type { get; set; }
    public string? FromUserId { get; set; }
    public User? FromUser { get; set; }
    public int? ThreadId { get; set; }
    public Thread? Thread { get; set; }
    public int? CommentId { get; set; }
    public Comment? Comment { get; set; }
    public int? MessageId { get; set; }
    public Message? Message { get; set; }
    public bool isRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}