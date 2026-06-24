namespace SkyRoute.Domain;

/// <summary>
/// Money value object. Currency amounts use <see cref="decimal"/> (never double/float) so pricing
/// arithmetic is exact. Construction is funnelled through factories that enforce the invariants:
/// amounts are non-negative and a single currency is used per value.
/// </summary>
public readonly record struct Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Money cannot be negative.");

        Amount = amount;
        Currency = currency;
    }

    public static Money Usd(decimal amount) => new(amount, Currency.USD);

    public static readonly Money ZeroUsd = Usd(0m);

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    /// <summary>
    /// Scales an amount by a decimal factor (e.g. base * 1.15). Rule-internal only: the result is
    /// always handed to <see cref="Pricing.Rounding.To2"/> before leaving a pricing rule. Does not round.
    /// </summary>
    public static Money operator *(Money money, decimal factor) =>
        new(money.Amount * factor, money.Currency);

    /// <summary>
    /// Multiplies a per-passenger amount by a passenger count. This is the deliberate "multiply last"
    /// step of the pricing pipeline; the per-passenger amount is already rounded, so this performs
    /// NO rounding by design.
    /// </summary>
    public static Money operator *(Money money, int count) =>
        new(money.Amount * count, money.Currency);

    public static Money Max(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount >= b.Amount ? a : b;
    }

    public override string ToString() => $"{Currency} {Amount:0.00}";

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException($"Currency mismatch: {a.Currency} vs {b.Currency}.");
    }
}
