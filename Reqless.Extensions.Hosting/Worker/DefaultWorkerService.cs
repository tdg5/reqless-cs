using Microsoft.Extensions.Hosting;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Background service for running a worker.
/// </summary>
public class DefaultWorkerService : BackgroundService
{
    /// <summary>
    /// A <see cref="IServiceProvider"/> instance that is used to create a new
    /// scope when creating worker instances.
    /// </summary>
    protected readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Factory for creating instances of <see cref="IWorker"/>.
    /// </summary>
    protected readonly IWorkerFactory _workerFactory;

    /// <summary>
    /// Create an instance of <see cref="DefaultWorkerService"/>.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance
    /// that is used to create a new scope when creating worker instances.</param>
    /// <param name="workerFactory">A <see cref="IWorkerFactory"/> instance that
    /// is used to create worker instances.</param>
    public DefaultWorkerService(
        IServiceProvider serviceProvider,
        IWorkerFactory workerFactory
    )
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(workerFactory, nameof(workerFactory));

        _serviceProvider = serviceProvider;
        _workerFactory = workerFactory;
    }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IWorker worker = _workerFactory.Create(_serviceProvider);
        return worker.ExecuteAsync(cancellationToken);
    }
}
