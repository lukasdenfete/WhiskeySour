using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.DataLayer;

public class Thread
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public string CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public byte[]? Image { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool isEdited => EditedAt.HasValue;
        
}