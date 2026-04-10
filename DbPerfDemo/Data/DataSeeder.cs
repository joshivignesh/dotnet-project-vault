using System.Net.Http.Headers;
using Bogus;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using DbPerfDemo.Models;

namespace DbPerfDemo.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync()) return; // Data already seeded
        
        var faker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Category, f => f.PickRandom(new [] { "Electronics", "Books", "Clothing", "Home", "Toys"} ))
            .RuleFor(p => p.Price, f => f.Random.Decimal(5, 2000))
            .RuleFor(p=> p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.CreatedDate, f => f.Date.Past(2));

            var Products = faker.Generate(100000);

            await context.AddRangeAsync(Products);
            await context.SaveChangesAsync();

    }
}