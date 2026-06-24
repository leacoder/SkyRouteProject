using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Tests.Pricing;

public class RoundingModeTests
{
    // Genuine discriminators between AwayFromZero (HALF-UP) and ToEven (banker's): each tie sits on an
    // exact half-cent whose nearest-even neighbour differs, so a silent switch to ToEven would fail here.
    [Theory]
    [InlineData(0.125, 0.13)] // ToEven would give 0.12 (down-to-even direction)
    [InlineData(0.145, 0.15)] // ToEven would give 0.14 (up-to-even direction)
    public void Rounds_half_away_from_zero_in_both_midpoint_directions(decimal raw, decimal expected)
    {
        Assert.Equal(Money.Usd(expected), Rounding.To2(Money.Usd(raw)));
    }
}
