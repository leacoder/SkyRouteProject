using Microsoft.Extensions.Logging.Abstractions;
using SkyRoute.Domain.Pricing;
using SkyRoute.Domain.Providers;

namespace SkyRoute.Domain.Tests;

public class FlightSearchServiceTests
{
    private static readonly Airport Jfk = new("JFK", "US", "New York");
    private static readonly Airport Lhr = new("LHR", "GB", "London");

    private static SearchCriteria Criteria() =>
        new(Jfk, Lhr, new DateOnly(2026, 7, 15), passengerCount: 2, CabinClass.Economy);

    private static Flight MakeFlight(string id, decimal baseFare) =>
        new(id, "Fake", id, Jfk, Lhr, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(7),
            CabinClass.Economy, Money.Usd(baseFare));

    [Fact]
    public async Task Aggregates_results_from_all_providers()
    {
        var service = new FlightSearchService(
            [new FakeProvider("A", [MakeFlight("A1", 100m)]), new FakeProvider("B", [MakeFlight("B1", 200m)])],
            NullLogger<FlightSearchService>.Instance);

        var results = await service.SearchAsync(Criteria(), CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Flight.Id == "A1");
        Assert.Contains(results, r => r.Flight.Id == "B1");
    }

    [Fact]
    public async Task A_failing_provider_does_not_sink_the_others()
    {
        var service = new FlightSearchService(
            [new FakeProvider("Bad", [], throws: true), new FakeProvider("Good", [MakeFlight("G1", 100m)])],
            NullLogger<FlightSearchService>.Instance);

        var results = await service.SearchAsync(Criteria(), CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("G1", results[0].Flight.Id);
    }

    private sealed class FakeProvider(string name, IReadOnlyList<Flight> flights, bool throws = false) : IFlightProvider
    {
        public string Name => name;
        public IPricingRule PricingRule { get; } = new GlobalAirPricingRule();

        public Task<IReadOnlyList<Flight>> FetchAsync(SearchCriteria criteria, CancellationToken cancellationToken) =>
            throws ? throw new InvalidOperationException("provider down") : Task.FromResult(flights);
    }
}
