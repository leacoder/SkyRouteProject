using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyRoute.Domain;

namespace SkyRoute.Api;

/// <summary>
/// Translates domain exceptions into RFC-9457 ProblemDetails: a document-format failure becomes a 422
/// with a <c>documentNumber</c> field error; any other domain rule violation (e.g. unknown airport,
/// stale flight, price mismatch) becomes a 400. Anything else is left to the default handler (500).
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;
        switch (exception)
        {
            case DocumentValidationException documentError:
                problem = new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["documentNumber"] = [documentError.Message],
                })
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Document validation failed.",
                };
                break;

            case DomainException domainError:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = domainError.Message,
                };
                break;

            default:
                return false; // not a domain exception — fall through to the default ProblemDetails handler.
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(
            problem, problem.GetType(), options: null, contentType: "application/problem+json", cancellationToken);
        return true;
    }
}
