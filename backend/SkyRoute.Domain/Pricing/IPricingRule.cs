namespace SkyRoute.Domain.Pricing;

/// <summary>
/// SEAM #1 — the business core. Takes a provider's BASE fare and returns the fully-resolved
/// per-passenger price with the provider's surcharge/discount/floor applied. Pure; it knows nothing
/// about passenger count (that is <see cref="Price"/>'s job).
/// <para>
/// CONTRACT: the returned <see cref="PricePerPassenger.DiscountableFare"/> (and any component) is
/// ALREADY rounded to 2 decimals; callers never round again.
/// </para>
/// </summary>
public interface IPricingRule
{
    PricePerPassenger PriceFor(Money baseFare);
}
