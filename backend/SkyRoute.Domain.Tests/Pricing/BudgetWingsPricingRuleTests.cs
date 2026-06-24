using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Tests.Pricing;

public class BudgetWingsPricingRuleTests
{
    private readonly BudgetWingsPricingRule _rule = new();

    [Theory]
    [InlineData(200.00, 180.00)] // 200 * 0.90, above the floor
    [InlineData(30.00, 29.99)]   // 27.00 -> floored up to 29.99
    [InlineData(33.32, 29.99)]   // 29.988 -> round2 -> 29.99 (== floor)
    [InlineData(33.33, 30.00)]   // 29.997 -> round2 -> 30.00 (ABOVE floor: rounding happens before the floor)
    public void Discounts_10_percent_on_base_then_applies_per_ticket_floor(decimal baseFare, decimal expectedPerPassenger)
    {
        var price = _rule.PriceFor(Money.Usd(baseFare));

        Assert.Equal(expectedPerPassenger, price.Amount.Amount);
    }
}
