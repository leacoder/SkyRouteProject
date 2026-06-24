using SkyRoute.Domain;

namespace SkyRoute.Api.Contracts;

/// <summary>Hand-written mapping between domain types and wire DTOs (no AutoMapper — the surface is small).</summary>
internal static class Mapping
{
    public static SearchCriteria ToCriteria(this SearchRequest request, AirportCatalog catalog) =>
        new(catalog.Get(request.OriginCode), catalog.Get(request.DestinationCode),
            request.DepartureDate, request.Passengers, request.Cabin);

    public static FlightResultDto ToDto(this FlightResult result) =>
        new(result.Flight.Id,
            result.Flight.ProviderName,
            result.Flight.FlightNumber,
            result.Flight.DepartureTime,
            result.Flight.ArrivalTime,
            (int)result.Flight.Duration.TotalMinutes,
            result.Flight.Cabin.ToString(),
            result.Price.PerPassengerAmount.Amount,
            result.Price.Total.Amount,
            result.Price.PassengerCount);

    public static AirportDto ToDto(this Airport airport) =>
        new(airport.IataCode, airport.City, airport.CountryCode);

    public static BookingResponse ToResponse(this Booking booking) =>
        new(booking.ReferenceCode,
            booking.Flight.ProviderName,
            booking.Flight.FlightNumber,
            booking.DocumentType.ToString(),
            booking.Price.PerPassengerAmount.Amount,
            booking.Price.Total.Amount,
            booking.Price.PassengerCount,
            booking.Price.Total.Currency.ToString());
}
