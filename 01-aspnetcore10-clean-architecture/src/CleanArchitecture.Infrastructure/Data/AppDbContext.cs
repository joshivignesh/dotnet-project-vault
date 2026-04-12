using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Category);
        });
    }

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var products = new[]
        {
            Product.Create("Dell XPS 15", "Premium 15-inch laptop with OLED display", 1899.99m, "Laptops", 25),
            Product.Create("Apple MacBook Pro M3", "Apple Silicon powerhouse for developers", 2499.99m, "Laptops", 15),
            Product.Create("Sony WH-1000XM5", "Industry-leading noise cancelling headphones", 349.99m, "Audio", 80),
            Product.Create("Samsung 4K Monitor 32\"", "Ultra-wide 4K HDR professional monitor", 699.99m, "Monitors", 40),
            Product.Create("Logitech MX Keys", "Wireless illuminated keyboard for multi-device", 119.99m, "Accessories", 150),
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}
