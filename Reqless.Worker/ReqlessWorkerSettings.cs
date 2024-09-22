using System.Collections.ObjectModel;
using Reqless.Common.Validation;

namespace Reqless.Worker;

/// <summary>
/// Base class containing common settings for the Reqless worker.
/// </summary>
public class ReqlessWorkerSettings
{
    /// <summary>
    /// The queue identifiers controlling what queues the worker(s) should take
    /// work from.
    /// </summary>
    public ReadOnlyCollection<string> QueueIdentifiers { get; }

    /// <summary>
    /// The number of workers to spawn.
    /// </summary>
    public int WorkerCount { get; }

    /// <summary>
    /// Create an instance of <see cref="ReqlessWorkerSettings"/>.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers controlling what
    /// queues the worker should take work from.</param>
    /// <param name="workerCount">The number of workers to spawn.</param>
    public ReqlessWorkerSettings(
        ReadOnlyCollection<string> queueIdentifiers,
        int workerCount
    )
    {
        ArgumentValidation.ThrowIfNullOrEmpty(queueIdentifiers, nameof(queueIdentifiers));
        ArgumentValidation.ThrowIfNotPositive(workerCount, nameof(workerCount));

        QueueIdentifiers = queueIdentifiers;
        WorkerCount = workerCount;
    }

}