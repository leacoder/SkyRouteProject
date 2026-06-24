namespace SkyRoute.Domain.Pricing;

/// <summary>
/// The single place where money is rounded to 2 decimal places. Uses HALF-UP
/// (<see cref="MidpointRounding.AwayFromZero"/>) — the convention a consumer expects for a displayed
/// fare (0.125 => 0.13). .NET's default banker's rounding (ToEven) is ledger-neutral but surprising
/// on an individual displayed price. Centralising it here means the mode is a one-line change and is
/// pinned by tests in both midpoint directions.
/// </summary>
public static class Rounding
{
    public static Money To2(Money money) =>
        Money.Usd(decimal.Round(money.Amount, 2, MidpointRounding.AwayFromZero));
}
