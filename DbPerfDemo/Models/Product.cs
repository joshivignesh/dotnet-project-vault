using Microsoft.EntityFrameworkCore;

namespace DbPerfDemo.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } ="";
    public string Category { get; set; } ="";
    public decimal Price { get; set; }
    public string Description { get; set; } ="";
    public DateTime CreatedDate { get; set; }

}

public class OrderItem 
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;   
    public decimal TotalPrice => Quantity * Price;
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = "";
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Create indexes for performance
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Category);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Price);

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Category, p.Price });

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderDate);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .HasIndex(oi => oi.ProductId);

        modelBuilder.Entity<OrderItem>()
            .HasIndex(oi => oi.OrderId);
    }
}