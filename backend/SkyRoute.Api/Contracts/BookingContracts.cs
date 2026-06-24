using System.ComponentModel.DataAnnotations;
using SkyRoute.Domain;

namespace SkyRoute.Api.Contracts;

/// <summary>
/// Wire model for a booking. Carries the route context so the server can independently re-derive the
/// document type AND re-price the flight. It deliberately does NOT carry a document type, and its price
/// (<see cref="DeclaredTotal"/>) is optional and only cross-checked — never trusted.
/// </summary>
public sealed class BookingRequest
{
    [Required] public string FlightId { get; init; } = string.Empty;
    [Required] public string OriginCode { get; init; } = string.Empty;
    [Required] public string DestinationCode { get; init; } = string.Empty;
    [Required] public DateOnly DepartureDate { get; init; }
    [Range(SearchCriteria.MinPassengers, SearchCriteria.MaxPassengers)] public int Passengers { get; init; }
    [Required] public CabinClass Cabin { get; init; }

    /// <summary>Optional. If present, the server rejects the booking when it disagrees with the re-priced total.</summary>
    public decimal? DeclaredTotal { get; init; }

    [Required] public PassengerDto Passenger { get; init; } = new();
}

public sealed class PassengerDto
{
    [Required] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string DocumentNumber { get; init; } = string.Empty;
}

public sealed record BookingResponse(
    string ReferenceCode,
    string Provider,
    string FlightNumber,
    string DocumentType,
    decimal PerPassenger,
    decimal Total,
    int PassengerCount,
    string Currency);
