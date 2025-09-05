namespace WhiskeySour.DataLayer;

public class Follow
{
    public int Id { get; set; }
    public string FollowerId { get; set; }
    public User Follower { get; set; }
    public string FolloweeId { get; set; }
    public User Followee { get; set; }
    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
}