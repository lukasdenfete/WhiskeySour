using Microsoft.EntityFrameworkCore;
using WhiskeySour.Domain;
namespace WhiskeySour.Infrastructure;

public class AppDbContext : DbContext 
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.ProductId);

        // Många-till-många mellan Order och User (utan explicit join-klass)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.UsersNavigation)
            .WithMany(u => u.OrdersNavigation)
            .UsingEntity<Dictionary<string, object>>(
                "UserOrders", // Namnet på den mellanliggande tabellen
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId")
            );
        // Många-till-många mellan Order och Product (utan explicit join-klass)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.ProductsNavigation)
            .WithMany(p => p.OrdersNavigation)
            .UsingEntity<Dictionary<string, object>>(
                "OrderProducts", // Namnet på den mellanliggande tabellen
                j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId")
            );
    }
    
}