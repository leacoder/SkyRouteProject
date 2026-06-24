namespace SkyRoute.Domain;

/// <summary>
/// Raised when a domain rule or invariant is violated (e.g. an unknown airport code, an invalid
/// route). The API maps this to an RFC-9457 ProblemDetails response.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
