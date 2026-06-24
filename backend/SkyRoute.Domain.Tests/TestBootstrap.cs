using System.Runtime.CompilerServices;

namespace SkyRoute.Domain.Tests;

internal static class TestBootstrap
{
    /// <summary>
    /// Forces a normal (execution-path) load of SkyRoute.Domain before xUnit's reflection-based discovery
    /// runs. On machines with Windows Smart App Control enforced, the reflection load of a freshly built,
    /// unsigned assembly can be blocked, while the ordinary execution load is allowed (it is the same path
    /// the running API uses). Touching a domain type here loads the assembly first, so discovery then finds
    /// it already loaded. A no-op everywhere else.
    /// </summary>
    [ModuleInitializer]
    public static void EnsureDomainAssemblyLoaded() => _ = Money.Usd(0m);
}
