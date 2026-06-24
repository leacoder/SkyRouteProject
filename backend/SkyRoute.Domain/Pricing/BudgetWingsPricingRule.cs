namespace SkyRoute.Domain.Pricing;

/// <summary>
/// BudgetWings pricing: base fare − 10% promotional discount (applied to the base fare only), with a
/// per-ticket minimum of $29.99. The floor is applied AFTER rounding the discounted fare and BEFORE
/// any non-discountable component would be added.
/// </summary>
public sealed class BudgetWingsPricingRule : IPricingRule
{
    private const decimal PromoDiscount = 0.10m;
    private static readonly Money MinimumFarePerTicket = Money.Usd(29.99m);

    public PricePerPassenger PriceFor(Money baseFare)
    {
        var discounted = Rounding.To2(baseFare * (1m - PromoDiscount));
        return PricePerPassenger.OfFare(Money.Max(discounted, MinimumFarePerTicket));
    }
}
