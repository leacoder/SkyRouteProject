using System.Text.RegularExpressions;

namespace SkyRoute.Domain;

/// <summary>
/// Pure route rules: which identity document a route requires and whether a document number is valid.
/// The backend is the source of truth — it derives the document type from the route's countries (never
/// from anything the client claims).
/// </summary>
public static partial class RouteRules
{
    public static DocumentType RequiredDocument(Airport origin, Airport destination) =>
        string.Equals(origin.CountryCode, destination.CountryCode, StringComparison.OrdinalIgnoreCase)
            ? DocumentType.NationalId   // same country  -> domestic
            : DocumentType.Passport;    // different country -> international

    public static bool IsValidDocument(DocumentType type, string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            return false;

        return type switch
        {
            DocumentType.Passport => PassportPattern().IsMatch(documentNumber),
            DocumentType.NationalId => NationalIdPattern().IsMatch(documentNumber),
            _ => false,
        };
    }

    [GeneratedRegex("^[A-Z][0-9]{7}$")]   // e.g. "A1234567"
    private static partial Regex PassportPattern();

    [GeneratedRegex("^[0-9]{8}$")]        // 8 digits
    private static partial Regex NationalIdPattern();
}
