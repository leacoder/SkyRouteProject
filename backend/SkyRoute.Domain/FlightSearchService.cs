using Microsoft.Extensions.Logging;
using SkyRoute.Domain.Pricing;
using SkyRoute.Domain.Providers;

namespace SkyRoute.Domain;

/// <summary>
/// Queries every registered <see cref="IFlightProvider"/> in parallel, prices each returned flight with
/// that provider's composed rule, and aggregates the results. It is closed for modification: it never
/// names a concrete provider, so adding an airline does not touch this class. It deliberately does NOT
/// sort — ordering is the frontend's responsibility (in memory, no re-fetch).
/// </summary>
public sealed class FlightSearchService(IEnumerable<IFlightProvider> providers, ILogger<FlightSearchService> logger)
{
    public async Task<IReadOnlyList<FlightResult>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken)
    {
        var perProvider = await Task.WhenAll(providers.Select(p => PriceProviderAsync(p, criteria, cancellationToken)));
        return perProvider.SelectMany(results => results).ToList();
    }

    private async Task<IReadOnlyList<FlightResult>> PriceProviderAsync(
        IFlightProvider provider, SearchCriteria criteria, CancellationToken cancellationToken)
    {
        try
        {
            var flights = await provider.FetchAsync(criteria, cancellationToken);
            return flights
                .Select(flight => new FlightResult(flight, Price.From(flight.BaseFare, provider.PricingRule, criteria.PassengerCount)))
                .ToList();
        }
        catch (Exception ex)
        {
            // One provider failing must not sink the whole search — skip its results and carry on.
            logger.LogWarning(ex, "Provider {Provider} failed; skipping its results.", provider.Name);
            return [];
        }
    }
}
