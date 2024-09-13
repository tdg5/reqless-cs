using Microsoft.Extensions.Hosting;

namespace Reqless.Worker;

/// <summary>
/// Background service for running a worker.
/// </summary>
public class WorkerService : BackgroundService
{
    /// <summary>
    /// Factory for creating instances of <see cref="IWorker"/>.
    /// </summary>
    protected readonly IWorkerFactory _workerFactory;

    /// <summary>
    /// Create an instance of <see cref="WorkerService"/>.
    /// </summary>
    public WorkerService(IWorkerFactory workerFactory)
    {
        _workerFactory = workerFactory;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IWorker worker = _workerFactory.Create();
        await worker.ExecuteAsync(cancellationToken);
    }
}