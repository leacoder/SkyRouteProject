using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Tests.Pricing;

public class PriceTests
{
    [Fact]
    public void Rounds_per_passenger_first_then_multiplies_for_total()
    {
        var price = Price.From(Money.Usd(200m), new GlobalAirPricingRule(), passengerCount: 2);

        Assert.Equal(230.00m, price.PerPassengerAmount.Amount);
        Assert.Equal(460.00m, price.Total.Amount);
    }

    [Fact]
    public void Applies_the_floor_per_ticket_before_multiplying()
    {
        // BudgetWings base 30 -> 29.99 per ticket -> 3 tickets -> 89.97 (not a single $29.99 order-level floor).
        var price = Price.From(Money.Usd(30m), new BudgetWingsPricingRule(), passengerCount: 3);

        Assert.Equal(29.99m, price.PerPassengerAmount.Amount);
        Assert.Equal(89.97m, price.Total.Amount);
    }

    [Fact]
    public void Does_not_re_round_the_total()
    {
        // GlobalAir 99.99 -> 114.99 per passenger; 114.99 * 3 = 344.97 exact (no second rounding).
        var price = Price.From(Money.Usd(99.99m), new GlobalAirPricingRule(), passengerCount: 3);

        Assert.Equal(344.97m, price.Total.Amount);
    }
}
