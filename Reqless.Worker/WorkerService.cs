using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reqless.Worker;

/// <summary>
/// Background service for running a worker.
/// </summary>
public class WorkerService : BackgroundService
{
    /// <summary>
    /// The name the worker should use when communicating with Reqless.
    /// </summary>
    protected string _name;

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
    /// <param name="name">The name the worker should use when communicating
    /// with Reqless.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance
    /// that is used to create a new scope when creating worker instances.</param>
    /// <param name="workerFactory">A <see cref="IWorkerFactory"/> instance that
    /// is used to create worker instances.</param>
    public WorkerService(
        string name,
        IServiceProvider serviceProvider,
        IWorkerFactory workerFactory
    )
    {
        _name = name;
        _serviceProvider = serviceProvider;
        _workerFactory = workerFactory;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IWorker worker = _workerFactory.Create(_serviceProvider, _name);
        await worker.ExecuteAsync(cancellationToken);
    }
}