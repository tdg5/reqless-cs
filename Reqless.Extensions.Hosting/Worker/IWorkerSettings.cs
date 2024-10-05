namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Interface for common settings for the Reqless worker.
/// </summary>
public interface IWorkerSettings
{
    /// <summary>
    /// The connection string to the Redis server.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// The queue identifiers controlling what queues the worker(s) should take
    /// work from.
    /// </summary>
    public IEnumerable<string> QueueIdentifiers { get; }

    /// <summary>
    /// The number of workers to spawn.
    /// </summary>
    public int WorkerCount { get; }

    /// <summary>
    /// The <see cref="IWorkerServiceRegistrar"/> instance to use when
    /// registering worker services with the application service collection.
    /// </summary>
    IWorkerServiceRegistrar WorkerServiceRegistrar { get; }
}
