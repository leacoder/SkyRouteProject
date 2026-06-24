namespace SkyRoute.Domain.Providers;

/// <summary>
/// Deterministic mock flight data. The real provider APIs are not accessible, so each provider returns a
/// realistic, <b>stable</b> set of flights for any search. Stability matters: a booking is re-priced
/// server-side by re-running the same search and matching the flight by id, so the same criteria must
/// always yield the same flights. Flights carry their BASE fare only; pricing is applied by the rules.
/// </summary>
internal static class MockFlights
{
    // Base fares are quoted for Economy; other cabins scale the base fare (this is part of the fare the
    // provider returns, before any surcharge/discount rule is applied).
    private static readonly IReadOnlyDictionary<CabinClass, decimal> CabinMultiplier =
        new Dictionary<CabinClass, decimal>
        {
            [CabinClass.Economy] = 1.0m,
            [CabinClass.Business] = 2.5m,
            [CabinClass.First] = 4.0m,
        };

    private sealed record Leg(string FlightNumber, int DepartureHour, int DurationMinutes, decimal BaseEconomyFare);

    private static readonly Leg[] GlobalAirSchedule =
    [
        new("GA413", 8, 400, 200m),
        new("GA117", 13, 380, 260m),
        new("GA805", 19, 520, 230m),
    ];

    private static readonly Leg[] BudgetWingsSchedule =
    [
        new("BW77", 6, 425, 150m),
        new("BW204", 11, 600, 180m),
        new("BW360", 22, 410, 90m),
    ];

    public static IReadOnlyList<Flight> GlobalAir(SearchCriteria criteria) =>
        Build("GlobalAir", GlobalAirSchedule, criteria);

    public static IReadOnlyList<Flight> BudgetWings(SearchCriteria criteria) =>
        Build("BudgetWings", BudgetWingsSchedule, criteria);

    private static IReadOnlyList<Flight> Build(string providerName, IEnumerable<Leg> schedule, SearchCriteria criteria)
    {
        var multiplier = CabinMultiplier[criteria.Cabin];

        return schedule.Select(leg =>
        {
            var departure = new DateTimeOffset(
                criteria.DepartureDate.ToDateTime(new TimeOnly(leg.DepartureHour, 0)), TimeSpan.Zero);
            var arrival = departure.AddMinutes(leg.DurationMinutes);
            var baseFare = Money.Usd(leg.BaseEconomyFare * multiplier);
            var id = $"{leg.FlightNumber}-{criteria.Origin.IataCode}{criteria.Destination.IataCode}-{criteria.DepartureDate:yyyyMMdd}";

            return new Flight(
                id, providerName, leg.FlightNumber,
                criteria.Origin, criteria.Destination,
                departure, arrival, criteria.Cabin, baseFare);
        }).ToList();
    }
}
