using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Providers;

/// <summary>BudgetWings provider: mocks flights and composes the BudgetWings pricing rule.</summary>
public sealed class BudgetWingsProvider : IFlightProvider
{
    public string Name => "BudgetWings";

    public IPricingRule PricingRule { get; } = new BudgetWingsPricingRule();

    public Task<IReadOnlyList<Flight>> FetchAsync(SearchCriteria criteria, CancellationToken cancellationToken) =>
        Task.FromResult(MockFlights.BudgetWings(criteria));
}
