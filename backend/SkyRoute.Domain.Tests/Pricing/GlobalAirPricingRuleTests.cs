using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Tests.Pricing;

public class GlobalAirPricingRuleTests
{
    private readonly GlobalAirPricingRule _rule = new();

    [Theory]
    [InlineData(200.00, 230.00)] // 200 * 1.15
    [InlineData(100.00, 115.00)] // 100 * 1.15
    [InlineData(99.99, 114.99)]  // 114.9885 -> HALF-UP -> 114.99
    public void Adds_15_percent_fuel_surcharge_and_rounds_to_2dp(decimal baseFare, decimal expectedPerPassenger)
    {
        var price = _rule.PriceFor(Money.Usd(baseFare));

        Assert.Equal(expectedPerPassenger, price.Amount.Amount);
    }
}
