using SonarGithubActions.Api.Models;
using SonarGithubActions.Api.Services;

namespace SonarGithubActions.Tests;

public class OrderCalculatorTests
{
    private readonly OrderCalculator _sut = new();

    // ── Basic calculation ────────────────────────────────────────────────────
    [Fact]
    public void Calculate_SingleItem_ReturnsCorrectTotal()
    {
        var request = new OrderRequest([new OrderItem("Widget", 100m, 2)]);
        var result = _sut.Calculate(request);

        Assert.Equal(200m, result.Subtotal);
        Assert.Equal(0m, result.DiscountPercent);
        Assert.Equal(16m, result.TaxAmount);    // 8% of 200
        Assert.Equal(216m, result.Total);
    }

    [Fact]
    public void Calculate_MultipleItems_SumsLineTotals()
    {
        var request = new OrderRequest([
            new OrderItem("A", 10m, 5),   // 50
            new OrderItem("B", 25m, 4),   // 100
        ]);
        var result = _sut.Calculate(request);

        Assert.Equal(150m, result.Subtotal);
    }

    // ── Tier discounts ───────────────────────────────────────────────────────
    [Theory]
    [InlineData(100,  0)]   // no tier
    [InlineData(500,  5)]   // tier 1
    [InlineData(1000, 10)]  // tier 2
    [InlineData(2500, 15)]  // tier 3
    [InlineData(5000, 20)]  // tier 4
    [InlineData(9999, 20)]  // stays at max tier
    public void Calculate_TierDiscount_AppliesCorrectPercent(decimal subtotal, decimal expectedDiscount)
    {
        var request = new OrderRequest([new OrderItem("Item", subtotal, 1)]);
        var result = _sut.Calculate(request);

        Assert.Equal(expectedDiscount, result.DiscountPercent);
    }

    // ── Promo codes ──────────────────────────────────────────────────────────
    [Fact]
    public void Calculate_ValidPromoCode_AppliesHigherOfPromoOrTier()
    {
        // Subtotal 600 → tier 5%, but SAVE10 gives 10%
        var request = new OrderRequest([new OrderItem("Item", 600m, 1)], PromoCode: "SAVE10");
        var result = _sut.Calculate(request);

        Assert.Equal(10m, result.DiscountPercent);
        Assert.Equal("SAVE10", result.AppliedPromoCode);
    }

    [Fact]
    public void Calculate_TierHigherThanPromo_TierWins()
    {
        // Subtotal 5000 → tier 20%; SAVE10 only gives 10%
        var request = new OrderRequest([new OrderItem("Item", 5000m, 1)], PromoCode: "SAVE10");
        var result = _sut.Calculate(request);

        Assert.Equal(20m, result.DiscountPercent);
    }

    [Fact]
    public void Calculate_UnknownPromoCode_IgnoresIt()
    {
        var request = new OrderRequest([new OrderItem("Item", 100m, 1)], PromoCode: "INVALID");
        var result = _sut.Calculate(request);

        Assert.Equal(0m, result.DiscountPercent);
        Assert.Null(result.AppliedPromoCode);
    }

    [Fact]
    public void Calculate_PromoCodeCaseInsensitive()
    {
        var request = new OrderRequest([new OrderItem("Item", 100m, 1)], PromoCode: "save20");
        var result = _sut.Calculate(request);

        Assert.Equal(20m, result.DiscountPercent);
    }

    // ── Validation ───────────────────────────────────────────────────────────
    [Fact]
    public void Calculate_EmptyItems_ThrowsArgumentException()
    {
        var request = new OrderRequest([]);
        Assert.Throws<ArgumentException>(() => _sut.Calculate(request));
    }

    [Fact]
    public void Calculate_NegativePrice_ThrowsArgumentException()
    {
        var request = new OrderRequest([new OrderItem("Bad", -1m, 1)]);
        Assert.Throws<ArgumentException>(() => _sut.Calculate(request));
    }

    [Fact]
    public void Calculate_ZeroQuantity_ThrowsArgumentException()
    {
        var request = new OrderRequest([new OrderItem("Item", 10m, 0)]);
        Assert.Throws<ArgumentException>(() => _sut.Calculate(request));
    }

    // ── Rounding ─────────────────────────────────────────────────────────────
    [Fact]
    public void Calculate_FractionalPrices_RoundsToTwoDecimals()
    {
        var request = new OrderRequest([new OrderItem("Item", 9.99m, 3)]);  // 29.97
        var result = _sut.Calculate(request);

        Assert.Equal(29.97m, result.Subtotal);
        // tax = 29.97 * 0.08 = 2.3976 → rounded to 2.40
        Assert.Equal(2.40m, result.TaxAmount);
    }
}
