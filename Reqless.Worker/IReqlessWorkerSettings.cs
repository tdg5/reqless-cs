using System.Collections.ObjectModel;

namespace Reqless.Worker;

/// <summary>
/// Interface for common settings for the Reqless worker.
/// </summary>
public interface IReqlessWorkerSettings
{
    /// <summary>
    /// The connection string to the Redis server.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// The queue identifiers controlling what queues the worker(s) should take
    /// work from.
    /// </summary>
    public ReadOnlyCollection<string> QueueIdentifiers { get; }

    /// <summary>
    /// The number of workers to spawn.
    /// </summary>
    public int WorkerCount { get; }
}
