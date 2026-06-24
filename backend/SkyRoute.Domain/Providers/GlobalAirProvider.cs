using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Providers;

/// <summary>GlobalAir provider: mocks flights and composes the GlobalAir pricing rule.</summary>
public sealed class GlobalAirProvider : IFlightProvider
{
    public string Name => "GlobalAir";

    public IPricingRule PricingRule { get; } = new GlobalAirPricingRule();

    public Task<IReadOnlyList<Flight>> FetchAsync(SearchCriteria criteria, CancellationToken cancellationToken) =>
        Task.FromResult(MockFlights.GlobalAir(criteria));
}
