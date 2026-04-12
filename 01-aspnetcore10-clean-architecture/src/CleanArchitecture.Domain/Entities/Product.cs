namespace CleanArchitecture.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Category { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() 
    {
        Name = string.Empty;
        Description = string.Empty;
        Category = string.Empty;
    }

    public static Product Create(string name, string description, decimal price, string category, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            StockQuantity = stockQuantity,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price, string category, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

        Name = name;
        Description = description;
        Price = price;
        Category = category;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
