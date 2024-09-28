namespace Reqless.Worker;

/// <summary>
/// Abstraction encapsulating the logic to repeatedly retrieve and execute jobs.
/// </summary>
public interface IWorker
{
    /// <summary>
    /// The name that the worker should use when communicating with Reqless.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Execute the worker.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// indicating whether or not the worker should discontinue work.</param>
    /// <returns>A task indicating when the worker has completed.</returns>
    public Task ExecuteAsync(CancellationToken cancellationToken);
}