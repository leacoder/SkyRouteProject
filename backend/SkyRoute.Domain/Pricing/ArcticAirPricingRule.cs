namespace SkyRoute.Domain.Pricing;

/// <summary>
/// ArcticAir pricing: base fare × 1.20, minus a flat $10 loyalty discount, with a per-ticket minimum
/// of $49.99. The discounted fare is rounded to 2dp BEFORE the floor is applied (same order as
/// BudgetWings). The flat discount is subtracted in decimal space and clamped at zero so it never
/// builds a negative Money; the floor then dominates for any low/negative intermediate.
/// </summary>
public sealed class ArcticAirPricingRule : IPricingRule
{
    private const decimal Markup = 1.20m;
    private const decimal LoyaltyDiscount = 10m;
    private static readonly Money MinimumFarePerTicket = Money.Usd(49.99m);

    public PricePerPassenger PriceFor(Money baseFare)
    {
        var afterLoyalty = (baseFare * Markup).Amount - LoyaltyDiscount;
        var discounted = Rounding.To2(Money.Usd(Math.Max(afterLoyalty, 0m)));
        return PricePerPassenger.OfFare(Money.Max(discounted, MinimumFarePerTicket));
    }
}
