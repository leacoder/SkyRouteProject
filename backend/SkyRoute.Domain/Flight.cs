namespace SkyRoute.Domain;

/// <summary>
/// A bookable flight as returned by a provider. Identified by <see cref="Id"/>; carries the provider's
/// <b>base</b> fare only (pricing is applied later by the provider's rule). It is a record, so a booking
/// can snapshot it and the snapshot is provably immutable.
/// </summary>
public sealed record Flight(
    string Id,
    string ProviderName,
    string FlightNumber,
    Airport Origin,
    Airport Destination,
    DateTimeOffset DepartureTime,
    DateTimeOffset ArrivalTime,
    CabinClass Cabin,
    Money BaseFare)
{
    public TimeSpan Duration => ArrivalTime - DepartureTime;
}
