using System.Collections.ObjectModel;
using Reqless.Common.Validation;

namespace Reqless.Worker;

/// <summary>
/// Base <see cref="IReqlessWorkerSettings"/> implementation containing common
/// settings for the Reqless worker.
/// </summary>
public class ReqlessWorkerSettings : IReqlessWorkerSettings
{
    /// <inheritdoc/>
    public string ConnectionString { get; }

    /// <inheritdoc/>
    public ReadOnlyCollection<string> QueueIdentifiers { get; }

    /// <inheritdoc/>
    public int WorkerCount { get; }

    /// <summary>
    /// Create an instance of <see cref="ReqlessWorkerSettings"/> using a
    /// default redis connection string.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers controlling what
    /// queues the worker should take work from.</param>
    /// <param name="workerCount">The number of workers to spawn.</param>
    public ReqlessWorkerSettings(
        ReadOnlyCollection<string> queueIdentifiers,
        int workerCount
    ) : this("localhost:6379", queueIdentifiers, workerCount)
    {
    }

    /// <summary>
    /// Create an instance of <see cref="ReqlessWorkerSettings"/>.
    /// </summary>
    /// <param name="connectionString">The connection string to the Reqless
    /// server. The semantics of the connection string depend on the client
    /// factory in use.</param>
    /// <param name="queueIdentifiers">The queue identifiers controlling what
    /// queues the worker should take work from.</param>
    /// <param name="workerCount">The number of workers to spawn.</param>
    public ReqlessWorkerSettings(
        string connectionString,
        ReadOnlyCollection<string> queueIdentifiers,
        int workerCount
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        ArgumentValidation.ThrowIfNullOrEmpty(queueIdentifiers, nameof(queueIdentifiers));
        ArgumentValidation.ThrowIfNotPositive(workerCount, nameof(workerCount));

        ConnectionString = connectionString;
        QueueIdentifiers = queueIdentifiers;
        WorkerCount = workerCount;
    }
}
