using Microsoft.Extensions.Hosting;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Background service for running a worker.
/// </summary>
public class DefaultWorkerService : BackgroundService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultWorkerService"/> class.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance
    /// that is used to create a new scope when creating worker instances.</param>
    /// <param name="workerFactory">A <see cref="IWorkerFactory"/> instance that
    /// is used to create worker instances.</param>
    public DefaultWorkerService(
        IServiceProvider serviceProvider,
        IWorkerFactory workerFactory)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(workerFactory, nameof(workerFactory));

        ServiceProvider = serviceProvider;
        WorkerFactory = workerFactory;
    }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance that is used to create
    /// a new scope when creating worker instances.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the factory for creating instances of <see cref="IWorker"/>.
    /// </summary>
    protected IWorkerFactory WorkerFactory { get; }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IWorker worker = WorkerFactory.Create(ServiceProvider);
        return worker.ExecuteAsync(cancellationToken);
    }
}
