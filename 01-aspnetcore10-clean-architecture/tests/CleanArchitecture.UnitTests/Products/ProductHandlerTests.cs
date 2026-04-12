using CleanArchitecture.Application.Products.Commands;
using CleanArchitecture.Application.Products.Queries;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.UnitTests.Products;

public class ProductHandlerTests : IAsyncLifetime
{
    private AppDbContext _context = null!;
    private IProductRepository _repository = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new ProductRepository(_context);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task CreateProduct_ShouldPersistAndReturnDto()
    {
        var handler = new CreateProductCommandHandler(_repository);
        var command = new CreateProductCommand("Test Laptop", "A great laptop", 999.99m, "Laptops", 10);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Laptop", result.Name);
        Assert.Equal(999.99m, result.Price);
        Assert.Equal(1, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task GetAllProducts_WithNoFilter_ShouldReturnAllProducts()
    {
        _context.Products.AddRange(
            Product.Create("Product A", "Desc A", 10m, "Cat1", 5),
            Product.Create("Product B", "Desc B", 20m, "Cat2", 10)
        );
        await _context.SaveChangesAsync();

        var handler = new GetAllProductsQueryHandler(_repository);
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllProducts_WithCategory_ShouldFilterCorrectly()
    {
        _context.Products.AddRange(
            Product.Create("Product A", "Desc A", 10m, "Electronics", 5),
            Product.Create("Product B", "Desc B", 20m, "Books", 10)
        );
        await _context.SaveChangesAsync();

        var handler = new GetAllProductsQueryHandler(_repository);
        var result = await handler.Handle(new GetAllProductsQuery("Electronics"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Product A", result[0].Name);
    }

    [Fact]
    public async Task GetProductById_WhenExists_ShouldReturnDto()
    {
        var product = Product.Create("Target Product", "Description", 150m, "Electronics", 20);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(_repository);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetProductById_WhenNotExists_ShouldReturnNull()
    {
        var handler = new GetProductByIdQueryHandler(_repository);
        var result = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProduct_WhenExists_ShouldReturnTrue()
    {
        var product = Product.Create("Old Name", "Old Desc", 100m, "Cat", 5);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(_repository);
        var command = new UpdateProductCommand(product.Id, "New Name", "New Desc", 200m, "Cat", 10);
        var success = await handler.Handle(command, CancellationToken.None);

        Assert.True(success);
        var updated = await _context.Products.FindAsync(product.Id);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(200m, updated.Price);
    }

    [Fact]
    public async Task DeleteProduct_WhenExists_ShouldReturnTrue()
    {
        var product = Product.Create("Delete Me", "Desc", 50m, "Test", 1);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(_repository);
        var success = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        Assert.True(success);
        Assert.Equal(0, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task DeleteProduct_WhenNotExists_ShouldReturnFalse()
    {
        var handler = new DeleteProductCommandHandler(_repository);
        var success = await handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(success);
    }
}
