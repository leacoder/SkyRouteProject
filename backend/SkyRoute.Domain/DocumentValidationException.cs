namespace SkyRoute.Domain;

/// <summary>
/// Raised when a passenger's document number does not match the format the route requires. The API maps
/// this to a 422 ProblemDetails with a <c>documentNumber</c> field error. Carries the server-derived
/// <see cref="RequiredDocument"/> so the message can name the exact expectation.
/// </summary>
public sealed class DocumentValidationException(DocumentType requiredDocument, Airport origin, Airport destination)
    : Exception(BuildMessage(requiredDocument, origin, destination))
{
    public DocumentType RequiredDocument { get; } = requiredDocument;

    private static string BuildMessage(DocumentType document, Airport origin, Airport destination) =>
        document == DocumentType.Passport
            ? $"International route {origin.IataCode}->{destination.IataCode} requires a valid Passport number ([A-Z][0-9]{{7}})."
            : $"Domestic route {origin.IataCode}->{destination.IataCode} requires a valid National ID (8 digits).";
}
