namespace SkyRoute.Domain.Pricing;

/// <summary>
/// The fully-resolved price for ONE passenger, split into:
/// <list type="bullet">
///   <item><see cref="DiscountableFare"/>: the base fare after the provider's surcharge/discount and
///   per-ticket floor, rounded to 2dp. This is the ONLY thing a provider rule ever touches.</item>
///   <item><see cref="Components"/>: today ALWAYS empty. Tomorrow: taxes/fees, each its own already-2dp
///   amount that the discount never applies to.</item>
/// </list>
/// Because the discount lives inside <see cref="DiscountableFare"/> and components are siblings the rule
/// never sees, "the discount cannot leak onto a tax" is a structural guarantee, not a comment.
/// </summary>
public readonly record struct PricePerPassenger
{
    public Money DiscountableFare { get; }
    public IReadOnlyList<PriceComponent> Components { get; }

    private PricePerPassenger(Money discountableFare, IReadOnlyList<PriceComponent> components)
    {
        DiscountableFare = discountableFare;
        Components = components;
    }

    /// <summary>Builds a per-passenger price from a discountable fare with no extra components (today's only path).</summary>
    public static PricePerPassenger OfFare(Money discountableFare) =>
        new(discountableFare, Array.Empty<PriceComponent>());

    /// <summary>
    /// Total for one passenger. Sums the already-2dp discountable fare and any already-2dp components
    /// (round-each-then-sum); it never re-rounds.
    /// </summary>
    public Money Amount =>
        Components.Aggregate(DiscountableFare, (running, component) => running + component.Amount);
}
