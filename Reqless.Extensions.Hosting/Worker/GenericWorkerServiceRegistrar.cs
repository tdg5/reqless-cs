using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reqless.Common.Validation;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Generic implementation of <see cref="IWorkerServiceRegistrar"/> that registers
/// <see cref="IHostedService"/> instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the <see cref="IHostedService"/> that should
/// be registered.</typeparam>
public class GenericWorkerServiceRegistrar<T>
    : IWorkerServiceRegistrar
    where T : class, IHostedService
{
    /// <inheritdoc/>
    public IServiceCollection RegisterWorkers(IServiceCollection services, int count)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentValidation.ThrowIfNegative(count, nameof(count));

        for (var index = 0; index < count; index++)
        {
            services.AddSingleton<IHostedService, T>();
        }

        return services;
    }
}
