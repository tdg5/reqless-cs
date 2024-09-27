using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reqless.Worker;

/// <summary>
/// Background service for running a worker.
/// </summary>
public class WorkerService : BackgroundService
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
    /// Create an instance of <see cref="WorkerService"/>.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance
    /// that is used to create a new scope when creating worker instances.</param>
    /// <param name="workerFactory">A <see cref="IWorkerFactory"/> instance that
    /// is used to create worker instances.</param>
    public WorkerService(
        IServiceProvider serviceProvider,
        IWorkerFactory workerFactory
    )
    {
        _serviceProvider = serviceProvider;
        _workerFactory = workerFactory;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        IWorker worker = _workerFactory.Create(scope.ServiceProvider);
        await worker.ExecuteAsync(cancellationToken);
    }
}