namespace WhiskeySour.Web.ViewModels;

public class MessageViewModel
{
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string ReceiverId { get; set; }
    public string ReceiverName { get; set; }
    public byte[]? SenderProfileImage { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsMine { get; set; }
    public bool IsRead { get; set; }
}