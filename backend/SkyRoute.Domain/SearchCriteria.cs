namespace SkyRoute.Domain;

/// <summary>
/// Immutable flight-search query. Built from already-resolved <see cref="Airport"/> value objects
/// (the API resolves IATA codes through the authoritative catalog first), so <see cref="IsInternational"/>
/// is trustworthy. Enforces the route and passenger-count invariants on construction.
/// </summary>
public sealed record SearchCriteria
{
    public const int MinPassengers = 1;
    public const int MaxPassengers = 9;

    public Airport Origin { get; }
    public Airport Destination { get; }
    public DateOnly DepartureDate { get; }
    public int PassengerCount { get; }
    public CabinClass Cabin { get; }

    public SearchCriteria(Airport origin, Airport destination, DateOnly departureDate, int passengerCount, CabinClass cabin)
    {
        if (origin == destination)
            throw new DomainException("Origin and destination must be different airports.");

        if (passengerCount is < MinPassengers or > MaxPassengers)
            throw new DomainException($"Passenger count must be between {MinPassengers} and {MaxPassengers}.");

        Origin = origin;
        Destination = destination;
        DepartureDate = departureDate;
        PassengerCount = passengerCount;
        Cabin = cabin;
    }

    /// <summary>A route is international when origin and destination are in different countries.</summary>
    public bool IsInternational =>
        !string.Equals(Origin.CountryCode, Destination.CountryCode, StringComparison.OrdinalIgnoreCase);
}
