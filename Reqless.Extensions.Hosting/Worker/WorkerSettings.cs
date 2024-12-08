using Reqless.Common.Validation;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Base <see cref="IWorkerSettings"/> implementation containing common
/// settings for the Reqless worker.
/// </summary>
public class WorkerSettings : IWorkerSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerSettings"/> class
    /// using the default redis connection string.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers controlling what
    /// queues the worker should take work from.</param>
    /// <param name="workerCount">The number of workers to spawn.</param>
    public WorkerSettings(
        IEnumerable<string> queueIdentifiers,
        int workerCount)
        : this("localhost:6379", queueIdentifiers, workerCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerSettings"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string to the Reqless
    /// server. The semantics of the connection string depend on the client
    /// factory in use.</param>
    /// <param name="queueIdentifiers">The queue identifiers controlling what
    /// queues the worker should take work from.</param>
    /// <param name="workerCount">The number of workers to spawn.</param>
    /// <param name="workerServiceRegistrar">The <see
    /// cref="IWorkerServiceRegistrar"/> to use when registering worker services
    /// with the application servicvice collection.</param>
    public WorkerSettings(
        string connectionString,
        IEnumerable<string> queueIdentifiers,
        int workerCount,
        IWorkerServiceRegistrar? workerServiceRegistrar = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        ArgumentValidation.ThrowIfNullOrEmpty(queueIdentifiers, nameof(queueIdentifiers));
        ArgumentValidation.ThrowIfNotPositive(workerCount, nameof(workerCount));

        ConnectionString = connectionString;
        QueueIdentifiers = queueIdentifiers;
        WorkerCount = workerCount;
        WorkerServiceRegistrar = workerServiceRegistrar
            ?? new DefaultWorkerServiceRegistrar();
    }

    /// <inheritdoc/>
    public string ConnectionString { get; }

    /// <inheritdoc/>
    public IEnumerable<string> QueueIdentifiers { get; }

    /// <inheritdoc/>
    public int WorkerCount { get; }

    /// <inheritdoc/>
    public IWorkerServiceRegistrar WorkerServiceRegistrar { get; }
}
