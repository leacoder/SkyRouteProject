using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain;

/// <summary>
/// A confirmed reservation, identified by its <see cref="ReferenceCode"/>. It snapshots the immutable
/// <see cref="Flight"/> record and the server-computed <see cref="Price"/>, so a later change elsewhere
/// can never mutate what was booked. <see cref="DocumentType"/> is the type the server derived and
/// validated against.
/// </summary>
public sealed record Booking(
    string ReferenceCode,
    Flight Flight,
    Passenger Passenger,
    DocumentType DocumentType,
    Price Price,
    DateTimeOffset CreatedAt);
