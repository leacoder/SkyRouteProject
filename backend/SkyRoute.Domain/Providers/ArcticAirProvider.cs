using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Providers;

/// <summary>ArcticAir provider: mocks flights and composes the ArcticAir pricing rule.</summary>
public sealed class ArcticAirProvider : IFlightProvider
{
    public string Name => "ArcticAir";

    public IPricingRule PricingRule { get; } = new ArcticAirPricingRule();

    public Task<IReadOnlyList<Flight>> FetchAsync(SearchCriteria criteria, CancellationToken cancellationToken) =>
        Task.FromResult(MockFlights.ArcticAir(criteria));
}
