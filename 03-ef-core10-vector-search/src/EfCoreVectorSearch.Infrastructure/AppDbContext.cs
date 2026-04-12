using System.Text.Json;
using EfCoreVectorSearch.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EfCoreVectorSearch.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<float[], string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => JsonSerializer.Deserialize<float[]>(value, (JsonSerializerOptions?)null) ?? Array.Empty<float>());

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Content).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(100).HasDefaultValue("General");
            entity.Property(x => x.Embedding).HasConversion(converter);
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.CreatedUtc);
        });
    }
}