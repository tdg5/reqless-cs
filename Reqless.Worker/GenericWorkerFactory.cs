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
    /// <returns>The <see cref="IWorker"/> instance.</returns>
    public IWorker Create(IServiceProvider serviceProvider) =>
        ActivatorUtilities.CreateInstance<T>(serviceProvider);
}