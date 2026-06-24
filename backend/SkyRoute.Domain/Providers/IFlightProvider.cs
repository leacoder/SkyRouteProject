using SkyRoute.Domain.Pricing;

namespace SkyRoute.Domain.Providers;

/// <summary>
/// SEAM #2 — a flight source. Fetches/mocks flights (returning their <b>base</b> fares) and exposes the
/// <see cref="IPricingRule"/> it <b>composes</b>. Fetching is deliberately separated from pricing.
/// <para>
/// Adding a new airline = a new class implementing this interface (composing its own rule) + one DI
/// registration. No existing code changes.
/// </para>
/// </summary>
public interface IFlightProvider
{
    string Name { get; }
    IPricingRule PricingRule { get; }
    Task<IReadOnlyList<Flight>> FetchAsync(SearchCriteria criteria, CancellationToken cancellationToken);
}
