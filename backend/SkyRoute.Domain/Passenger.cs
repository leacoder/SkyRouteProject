namespace SkyRoute.Domain;

/// <summary>The traveller's details captured at booking time. Pure data, no identity of its own.</summary>
public sealed record Passenger(string FullName, string Email, string DocumentNumber);
