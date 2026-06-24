namespace SkyRoute.Domain;

/// <summary>
/// The authoritative IATA-code → <see cref="Airport"/> map. The server owns this data, so it — not the
/// client — decides each airport's country, and therefore whether a route is international or domestic.
/// Hardcoded reference data (6+ airports across 5 countries); a concrete class, not an interface, because
/// there is no second implementation to justify a seam.
/// </summary>
public sealed class AirportCatalog
{
    private static readonly IReadOnlyList<Airport> AllAirports =
    [
        new("JFK", "US", "New York"),
        new("LAX", "US", "Los Angeles"),
        new("MIA", "US", "Miami"),
        new("LHR", "GB", "London"),
        new("CDG", "FR", "Paris"),
        new("FRA", "DE", "Frankfurt"),
        new("MAD", "ES", "Madrid"),
    ];

    private static readonly IReadOnlyDictionary<string, Airport> ByCode =
        AllAirports.ToDictionary(airport => airport.IataCode, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<Airport> All => AllAirports;

    public Airport Get(string iataCode) =>
        ByCode.TryGetValue(iataCode, out var airport)
            ? airport
            : throw new DomainException($"Unknown airport code '{iataCode}'.");
}
