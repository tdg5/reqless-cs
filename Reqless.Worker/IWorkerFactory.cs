namespace Reqless.Worker;

/// <summary>
/// Factory for creating instances of <see cref="IWorker"/>.
/// </summary>
public interface IWorkerFactory
{
    /// <summary>
    /// Create an <see cref="IWorker"/> instance.
    /// </summary>
    /// <returns>An <see cref="IWorker"/> instance.</returns>
    IWorker Create();
}