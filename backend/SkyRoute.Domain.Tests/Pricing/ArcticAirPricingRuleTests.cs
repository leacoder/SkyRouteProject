using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Tests.Pricing;

public class ArcticAirPricingRuleTests
{
    private readonly ArcticAirPricingRule _rule = new();

    [Theory]
    [InlineData(200.00, 230.00)] // 200*1.20=240, -10 = 230.00 (above floor)
    [InlineData(100.00, 110.00)] // 120 - 10 = 110.00
    [InlineData(99.99, 109.99)]  // 119.988 - 10 = 109.988 -> HALF-UP -> 109.99
    [InlineData(50.00, 50.00)]   // 60 - 10 = 50.00 (just above floor)
    [InlineData(49.00, 49.99)]   // 58.80 - 10 = 48.80 -> floored up to 49.99
    [InlineData(5.00, 49.99)]    // 6 - 10 = -4 (clamped to 0) -> floored to 49.99
    public void Marks_up_20_percent_subtracts_loyalty_then_applies_per_ticket_floor(
        decimal baseFare, decimal expectedPerPassenger)
    {
        var price = _rule.PriceFor(Money.Usd(baseFare));

        Assert.Equal(expectedPerPassenger, price.Amount.Amount);
    }
}
