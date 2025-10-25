using WhiskeySour.DataLayer;
using ThreadEntity = WhiskeySour.DataLayer.Thread;

namespace WhiskeySour.Web.ViewModels;

public class HomeViewModel
{
    public List<ThreadEntity> RecentThreads { get; set; }
    public List<ThreadEntity> PopularThreads { get; set; }
    public List<ThreadEntity> FollowerThreads { get; set; }
    public List<User> RecentUsers { get; set; }
    public List<User> RecommendedUsers { get; set; }
    public List<Product> RecentProducts { get; set; }
}