namespace SkyRoute.Domain.Pricing;

/// <summary>
/// A non-discountable per-passenger price component (e.g. a tax or fee) for the FUTURE. Unused today
/// — no component is ever constructed — but it defines the seam: a component's amount is stored
/// already rounded to 2dp, and a provider's discount never operates on it (the discount is frozen
/// inside <see cref="PricePerPassenger.DiscountableFare"/> first).
/// </summary>
public readonly record struct PriceComponent(string Label, Money Amount);
