namespace SonarGithubActions.Api.Models;

public record OrderItem(string ProductName, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public record OrderRequest(List<OrderItem> Items, string? PromoCode = null);

public record OrderSummary(
    List<OrderItem> Items,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal Total,
    string? AppliedPromoCode
);
