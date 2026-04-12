using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.Products.Queries;

public record GetAllProductsQuery(string? Category = null) : IRequest<IReadOnlyList<ProductDto>>;

public sealed class GetAllProductsQueryHandler(IProductRepository repository)
    : IRequestHandler<GetAllProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = string.IsNullOrWhiteSpace(request.Category)
            ? await repository.GetAllAsync(cancellationToken)
            : await repository.GetByCategoryAsync(request.Category, cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Price, p.Category, p.StockQuantity, p.CreatedAt, p.UpdatedAt
        )).ToList();
    }
}
