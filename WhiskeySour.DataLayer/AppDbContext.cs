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
    }
    
    
}