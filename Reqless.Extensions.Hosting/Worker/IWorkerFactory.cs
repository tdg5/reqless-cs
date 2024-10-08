namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Factory for creating instances of <see cref="IWorker"/>.
/// </summary>
public interface IWorkerFactory
{
    /// <summary>
    /// Create an <see cref="IWorker"/> instance with the given identifier.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance
    /// that can be utilized when creating worker instances.</param>
    /// <returns>An <see cref="IWorker"/> instance.</returns>
    IWorker Create(IServiceProvider serviceProvider);
}
