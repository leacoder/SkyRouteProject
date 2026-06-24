using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain;

/// <summary>A flight paired with its fully-resolved <see cref="Price"/> for the requested passenger count.</summary>
public sealed record FlightResult(Flight Flight, Price Price);
