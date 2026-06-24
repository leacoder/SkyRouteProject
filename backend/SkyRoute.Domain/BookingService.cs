using System.Collections.Concurrent;
using System.Globalization;

namespace SkyRoute.Domain;

/// <summary>
/// Confirms bookings. Crucially, the server never trusts the client's authority: it re-derives the
/// required document type from the route's countries, re-validates the document number, and RE-PRICES the
/// flight by re-running the search and matching by id (so the graded discount and $29.99 floor are
/// enforced here, not echoed from the client). Confirmed bookings are kept in an in-memory store.
/// A concrete class — there is no second implementation to justify an interface.
/// </summary>
public sealed class BookingService(FlightSearchService searchService)
{
    private readonly ConcurrentDictionary<string, Booking> _bookings = new();

    public async Task<Booking> ConfirmAsync(
        SearchCriteria criteria, string flightId, Passenger passenger, decimal? declaredTotal, CancellationToken cancellationToken)
    {
        // 1. Re-derive the required document from the authoritative route, then validate the number.
        var requiredDocument = RouteRules.RequiredDocument(criteria.Origin, criteria.Destination);
        if (!RouteRules.IsValidDocument(requiredDocument, passenger.DocumentNumber))
            throw new DocumentValidationException(requiredDocument, criteria.Origin, criteria.Destination);

        // 2. Re-price: re-run the search and match the chosen flight by id. The server price is authoritative.
        var results = await searchService.SearchAsync(criteria, cancellationToken);
        var match = results.FirstOrDefault(result => result.Flight.Id == flightId)
            ?? throw new DomainException($"Flight '{flightId}' is no longer available for the selected search.");

        // 3. If the client declared a total, it must agree with the server's re-priced total.
        if (declaredTotal is { } clientTotal && clientTotal != match.Price.Total.Amount)
            throw new DomainException(string.Format(CultureInfo.InvariantCulture,
                "Declared total {0:0.00} does not match the current price {1:0.00}.", clientTotal, match.Price.Total.Amount));

        // 4. Persist and return.
        var booking = new Booking(
            GenerateReference(), match.Flight, passenger, requiredDocument, match.Price, DateTimeOffset.UtcNow);
        _bookings[booking.ReferenceCode] = booking;
        return booking;
    }

    public bool TryGet(string referenceCode, out Booking? booking) =>
        _bookings.TryGetValue(referenceCode, out booking);

    private static string GenerateReference() => $"SR-{Guid.NewGuid():N}"[..9].ToUpperInvariant();
}
