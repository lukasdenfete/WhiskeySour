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
    public ICollection<Comment> Comments { get; set; }
    public byte[]? Image { get; set; }
    
}