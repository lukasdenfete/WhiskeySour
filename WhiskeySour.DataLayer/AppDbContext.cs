using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.DataLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WhiskeySour.DataLayer;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Thread> Threads { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)"); // Detta säkerställer att EF använder rätt kolumntyp
    
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        // Många-till-många mellan Order och User
        modelBuilder.Entity<Order>()
            .HasMany(o => o.UsersNavigation)
            .WithMany(u => u.OrdersNavigation) 
            .UsingEntity<Dictionary<string, object>>(
                "UserOrders", //Namnet på sambandstabellen
                j => j.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId") // UserId är FK i den sambandstabellen
                    .HasPrincipalKey(u => u.Id), // Id är PK i User-tabellen

                j => j.HasOne<Order>()
                    .WithMany()
                    .HasForeignKey("OrderId") // OrderId är FK i den sambandstabellen
                    .HasPrincipalKey(o => o.OrderId) // Id är PK i Order-tabellen
            );
        // Många-till-många mellan Order och Product
        modelBuilder.Entity<Order>()
            .HasMany(o => o.ProductsNavigation)
            .WithMany(p => p.OrdersNavigation)
            .UsingEntity<Dictionary<string, object>>(
                "OrderProducts", // Namnet på den sambandstabellen
                j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId")
            );
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Thread)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.ThreadId)
            .OnDelete(DeleteBehavior.ClientCascade); 

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.User)
            .WithMany()
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followee)
            .WithMany()
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.FromUser)
            .WithMany()
            .HasForeignKey(n => n.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Thread)
            .WithMany(t => t.Notifications)
            .HasForeignKey(n => n.ThreadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Comment)
            .WithMany(c => c.Notifications)
            .HasForeignKey(n => n.CommentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Message)
            .WithMany()
            .HasForeignKey(n => n.MessageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    
}