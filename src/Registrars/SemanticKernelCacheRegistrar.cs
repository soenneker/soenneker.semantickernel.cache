using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.SemanticKernel.Cache.Abstract;

namespace Soenneker.SemanticKernel.Cache.Registrars;

/// <summary>
/// Providing async thread-safe singleton Semantic Kernel instances
/// </summary>
public static class SemanticKernelCacheRegistrar
{
    /// <summary>
    /// Adds <see cref="ISemanticKernelCache"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddSemanticKernelCacheAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<ISemanticKernelCache, SemanticKernelCache>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ISemanticKernelCache"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddSemanticKernelCacheAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<ISemanticKernelCache, SemanticKernelCache>();

        return services;
    }
}
