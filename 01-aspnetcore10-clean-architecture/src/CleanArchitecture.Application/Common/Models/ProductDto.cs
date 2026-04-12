namespace CleanArchitecture.Application.Common.Models;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
