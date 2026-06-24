namespace SkyRoute.Domain.Pricing;

/// <summary>GlobalAir pricing: base fare + 15% fuel surcharge, rounded to 2 decimals.</summary>
public sealed class GlobalAirPricingRule : IPricingRule
{
    private const decimal FuelSurcharge = 0.15m;

    public PricePerPassenger PriceFor(Money baseFare) =>
        PricePerPassenger.OfFare(Rounding.To2(baseFare * (1m + FuelSurcharge)));
}
