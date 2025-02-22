using Microsoft.EntityFrameworkCore;
using WhiskeySour.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WhiskeySour.Infrastructure;

public class AppDbContext : IdentityDbContext<IdentityUser> 
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
                "UserOrders", // Namnet på den mellanliggande tabellen
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId")
            );
        // Många-till-många mellan Order och Product
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