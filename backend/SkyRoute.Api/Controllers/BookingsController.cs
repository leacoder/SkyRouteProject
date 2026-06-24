using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Domain;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public sealed class BookingsController(AirportCatalog catalog, BookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Confirms a booking. The server re-derives the document type from the route, re-validates the
    /// document number, and re-prices the flight before persisting — the client is never trusted.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingRequest request, CancellationToken cancellationToken)
    {
        var criteria = new SearchCriteria(
            catalog.Get(request.OriginCode), catalog.Get(request.DestinationCode),
            request.DepartureDate, request.Passengers, request.Cabin);

        var passenger = new Passenger(request.Passenger.FullName, request.Passenger.Email, request.Passenger.DocumentNumber);

        var booking = await bookingService.ConfirmAsync(
            criteria, request.FlightId, passenger, request.DeclaredTotal, cancellationToken);

        return CreatedAtAction(nameof(GetByReference), new { referenceCode = booking.ReferenceCode }, booking.ToResponse());
    }

    /// <summary>Retrieves a previously confirmed booking from the in-memory store.</summary>
    [HttpGet("{referenceCode}")]
    public ActionResult<BookingResponse> GetByReference(string referenceCode) =>
        bookingService.TryGet(referenceCode, out var booking)
            ? Ok(booking!.ToResponse())
            : NotFound();
}
