using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reqless.Worker;

/// <summary>
/// Abstraction for registering <see cref="IHostedService"/> instances.
/// </summary>
public interface IWorkerServiceRegistrar
{
    /// <summary>
    /// Registers <paramref name="count"/> worker services with the given
    /// <paramref name="services"/> <see cref="IServiceCollection"/> instance.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance
    /// that worker services should be registered with.</param>
    /// <param name="count">The number of worker services that should be
    /// registered.</param>
    /// <returns>The given <paramref name="services"/> <see
    /// cref="IServiceCollection"/> instance.</returns>
    public IServiceCollection RegisterWorkers(IServiceCollection services, int count);
}
