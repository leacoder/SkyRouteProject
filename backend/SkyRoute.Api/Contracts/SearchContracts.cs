using System.ComponentModel.DataAnnotations;
using SkyRoute.Domain;

namespace SkyRoute.Api.Contracts;

/// <summary>Wire model for a flight search. The client sends IATA codes only — never a country or price.</summary>
public sealed class SearchRequest
{
    [Required] public string OriginCode { get; init; } = string.Empty;
    [Required] public string DestinationCode { get; init; } = string.Empty;
    [Required] public DateOnly DepartureDate { get; init; }
    [Range(SearchCriteria.MinPassengers, SearchCriteria.MaxPassengers)] public int Passengers { get; init; }
    [Required] public CabinClass Cabin { get; init; }
}

/// <summary>A single priced flight. Both <see cref="PerPassenger"/> and <see cref="Total"/> are returned.</summary>
public sealed record FlightResultDto(
    string Id,
    string Provider,
    string FlightNumber,
    DateTimeOffset DepartureTime,
    DateTimeOffset ArrivalTime,
    int DurationMinutes,
    string Cabin,
    decimal PerPassenger,
    decimal Total,
    int PassengerCount);

/// <summary>Search response. Document context is included so the booking screen can be server-driven.</summary>
public sealed record SearchResponse(
    bool IsInternational,
    string RequiredDocumentType,
    string Currency,
    IReadOnlyList<FlightResultDto> Results);
