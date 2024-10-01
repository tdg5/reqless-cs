using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds essential Reqless.Worker services to the specified <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add
    /// services to.</param>
    /// <param name="settings">The <see cref="ReqlessWorkerSettings"/> instance
    /// that should be used to configure the worker services.</param>
    /// <returns>Returns the given <see cref="IServiceCollection"/>
    /// instance.</returns>
    public static IServiceCollection AddReqlessWorkerServices(
        this IServiceCollection services,
        ReqlessWorkerSettings settings
    )
    {
        services.AddReqlessServices();
        services.AddSingleton<IReqlessWorkerSettings>(settings);
        services.TryAddSingleton<IJobContextAccessor, DefaultJobContextAccessor>();
        services.TryAddSingleton<IJobContextFactory, DefaultJobContextFactory>();
        services.TryAddSingleton<IJobReserver, DefaultJobReserver>();
        services.TryAddSingleton<IQueueIdentifierResolver, DefaultQueueIdentifierResolver>();
        services.TryAddSingleton<IQueueNameProvider, DefaultQueueNameProvider>();
        services.TryAddSingleton<IReqlessClientFactory, DefaultReqlessClientFactory>();
        services.TryAddSingleton<IUnitOfWorkActivator, DefaultUnitOfWorkActivator>();
        services.TryAddSingleton<IUnitOfWorkResolver, DefaultUnitOfWorkResolver>();
        services.TryAddSingleton<IWorkerFactory, GenericWorkerFactory<AsyncWorker>>();
        services.TryAddSingleton<IWorkerNameProvider, DefaultWorkerNameProvider>();
        settings.WorkerServiceRegistrar.RegisterWorkers(services, settings.WorkerCount);

        return services;
    }
}
