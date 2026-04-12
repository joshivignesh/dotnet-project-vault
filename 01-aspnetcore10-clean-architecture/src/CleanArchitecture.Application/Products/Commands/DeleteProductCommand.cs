using CleanArchitecture.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteProductCommandHandler(IProductRepository repository)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsAsync(request.Id, cancellationToken);
        if (!exists) return false;

        await repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
