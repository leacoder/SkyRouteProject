using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Domain;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/flights")]
public sealed class FlightsController(AirportCatalog catalog, FlightSearchService searchService) : ControllerBase
{
    /// <summary>
    /// Searches every provider and returns priced, UNSORTED results (the client sorts in memory). POST is
    /// used because the payload is a structured, evolving object; results are live-priced so caching a GET
    /// would buy nothing.
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var criteria = request.ToCriteria(catalog);
        var results = await searchService.SearchAsync(criteria, cancellationToken);
        var requiredDocument = RouteRules.RequiredDocument(criteria.Origin, criteria.Destination);

        return Ok(new SearchResponse(
            criteria.IsInternational,
            requiredDocument.ToString(),
            Currency.USD.ToString(),
            results.Select(result => result.ToDto()).ToList()));
    }
}
