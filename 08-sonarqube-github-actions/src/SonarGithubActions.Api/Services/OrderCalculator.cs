using SonarGithubActions.Api.Models;

namespace SonarGithubActions.Api.Services;

/// <summary>
/// Calculates order totals with tier-based discounts, promo codes, and tax.
/// Demonstrated for SonarCloud code quality analysis.
/// </summary>
public class OrderCalculator
{
    private const decimal TaxRate = 0.08m; // 8%

    private static readonly Dictionary<string, decimal> PromoCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "SAVE10", 10m },
        { "SAVE20", 20m },
        { "VIP30",  30m }
    };

    // Tier discounts: subtotal threshold → discount percentage
    public static readonly IReadOnlyList<(decimal Threshold, decimal DiscountPercent)> DiscountTiers =
    [
        (500m, 5m),
        (1000m, 10m),
        (2500m, 15m),
        (5000m, 20m)
    ];

    public OrderSummary Calculate(OrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("Order must have at least one item.", nameof(request));

        foreach (var item in request.Items)
        {
            if (item.UnitPrice < 0)
                throw new ArgumentException($"Item '{item.ProductName}' has a negative price.", nameof(request));
            if (item.Quantity <= 0)
                throw new ArgumentException($"Item '{item.ProductName}' must have a positive quantity.", nameof(request));
        }

        var subtotal = request.Items.Sum(i => i.LineTotal);
        var (discountPercent, appliedPromo) = ResolveDiscount(subtotal, request.PromoCode);
        var discountAmount = Math.Round(subtotal * discountPercent / 100m, 2);
        var discountedSubtotal = subtotal - discountAmount;
        var taxAmount = Math.Round(discountedSubtotal * TaxRate, 2);
        var total = discountedSubtotal + taxAmount;

        return new OrderSummary(
            Items: request.Items,
            Subtotal: Math.Round(subtotal, 2),
            DiscountPercent: discountPercent,
            DiscountAmount: discountAmount,
            TaxAmount: taxAmount,
            Total: Math.Round(total, 2),
            AppliedPromoCode: appliedPromo
        );
    }

    private static (decimal discountPercent, string? appliedPromo) ResolveDiscount(decimal subtotal, string? promoCode)
    {
        // Promo code takes priority — but only if it's higher than the tier discount
        if (!string.IsNullOrWhiteSpace(promoCode) && PromoCodes.TryGetValue(promoCode, out var promoDiscount))
        {
            var tierDiscount = GetTierDiscount(subtotal);
            return (Math.Max(promoDiscount, tierDiscount), promoCode.ToUpperInvariant());
        }

        return (GetTierDiscount(subtotal), null);
    }

    private static decimal GetTierDiscount(decimal subtotal)
    {
        var discount = 0m;
        foreach (var (threshold, percent) in DiscountTiers)
        {
            if (subtotal >= threshold)
                discount = percent;
        }
        return discount;
    }
}
