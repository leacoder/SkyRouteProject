namespace SkyRoute.Domain.Pricing;

/// <summary>
/// Price value object for a flight result / booking. The single factory <see cref="From"/> is the ONLY
/// way to build it, which forces the correct pricing order: the rule computes (and rounds) the
/// per-passenger price FIRST, and the total is derived by multiplying by the passenger count LAST.
/// There is no constructor that accepts a total, so "round(perPax * count)" or "discount after multiply"
/// are unrepresentable.
/// </summary>
public readonly record struct Price
{
    public PricePerPassenger PerPassenger { get; }
    public int PassengerCount { get; }

    private Price(PricePerPassenger perPassenger, int passengerCount)
    {
        if (passengerCount is < SearchCriteria.MinPassengers or > SearchCriteria.MaxPassengers)
            throw new ArgumentOutOfRangeException(nameof(passengerCount), passengerCount,
                $"Passenger count must be between {SearchCriteria.MinPassengers} and {SearchCriteria.MaxPassengers}.");

        PerPassenger = perPassenger;
        PassengerCount = passengerCount;
    }

    public static Price From(Money baseFare, IPricingRule rule, int passengerCount) =>
        new(rule.PriceFor(baseFare), passengerCount);

    /// <summary>Per-passenger total — already rounded to 2dp by the rule.</summary>
    public Money PerPassengerAmount => PerPassenger.Amount;

    /// <summary>
    /// Grand total for all passengers. Multiplies an already-2dp per-passenger amount by an integer
    /// count, so the result is exact and is NOT rounded again.
    /// </summary>
    public Money Total => PerPassenger.Amount * PassengerCount;
}
