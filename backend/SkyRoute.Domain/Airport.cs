namespace SkyRoute.Domain;

/// <summary>
/// Airport value object. Identified by its IATA code and carries its ISO country code, so any code
/// that needs the country (e.g. the international-vs-domestic decision) reads it straight from the
/// airport instead of re-deriving it and risking a stale pairing.
/// </summary>
public readonly record struct Airport(string IataCode, string CountryCode, string City);
