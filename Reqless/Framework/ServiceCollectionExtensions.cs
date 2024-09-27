using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Reqless.Client;

namespace Reqless.Framework;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds essential Reqless services to the specified <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add
    /// services to.</param>
    /// <returns>Returns the given <see cref="IServiceCollection"/>
    /// instance.</returns>
    public static IServiceCollection AddReqlessServices(
        this IServiceCollection services
    )
    {
        services.TryAddSingleton<IReqlessClient>(
            provider => provider.GetRequiredService<IReqlessClientFactory>().Create()
        );
        services.TryAddSingleton<IUnitOfWorkActivator, DefaultUnitOfWorkActivator>();
        services.TryAddSingleton<IUnitOfWorkResolver, DefaultUnitOfWorkResolver>();
        services.TryAddSingleton<IJobContextAccessor, DefaultJobContextAccessor>();
        services.TryAddSingleton<IJobContextFactory, DefaultJobContextFactory>();

        return services;
    }
}
