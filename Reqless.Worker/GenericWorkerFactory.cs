using Microsoft.Extensions.DependencyInjection;

namespace Reqless.Worker;

/// <summary>
/// Implementation of <see cref="IWorkerFactory"/> that creates <see
/// cref="IWorker"/> instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericWorkerFactory<T> : IWorkerFactory where T : IWorker
{
    /// <summary>
    /// Create an <see cref="IWorker"/> instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance
    /// that can be utilized when creating worker instances.</param>
    /// <returns>The <see cref="IWorker"/> instance.</returns>
    /// <param name="name">The name that should be given to the worker for use when communicating
    /// with Reqless.</param>
    public IWorker Create(IServiceProvider serviceProvider, string name) =>
        ActivatorUtilities.CreateInstance<T>(serviceProvider, name);
}
