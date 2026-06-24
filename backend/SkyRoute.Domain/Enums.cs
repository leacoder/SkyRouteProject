namespace SkyRoute.Domain;

/// <summary>Cabin classes a passenger can book. Closed set, no behaviour — hence an enum, not a value object.</summary>
public enum CabinClass
{
    Economy,
    Business,
    First
}

/// <summary>The kind of identity document a route requires. Derived server-side from the route's countries.</summary>
public enum DocumentType
{
    Passport,
    NationalId
}

/// <summary>Supported currencies. Single-currency today (USD); modelled so money always carries its currency.</summary>
public enum Currency
{
    USD
}
