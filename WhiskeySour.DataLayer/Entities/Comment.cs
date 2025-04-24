using System.ComponentModel.DataAnnotations;

namespace WhiskeySour.DataLayer;

public class Comment
{
    public int Id { get; set; }
    [Required]
    public string Content { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public int ThreadId { get; set; }
    public Thread Thread { get; set; }
    public string CreatedById { get; set; }
    public User CreatedBy { get; set; }
}