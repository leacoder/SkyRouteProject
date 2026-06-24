using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.Contracts;
using SkyRoute.Domain;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/airports")]
public sealed class AirportsController(AirportCatalog catalog) : ControllerBase
{
    /// <summary>Static reference data for the search dropdowns (GET — cacheable, no body).</summary>
    [HttpGet]
    public ActionResult<IReadOnlyList<AirportDto>> GetAll() =>
        Ok(catalog.All.Select(airport => airport.ToDto()).ToList());
}
