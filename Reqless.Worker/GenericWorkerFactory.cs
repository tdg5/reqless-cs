using Microsoft.Extensions.DependencyInjection;

namespace Reqless.Worker;

/// <summary>
/// Implementation of <see cref="IWorkerFactory"/> that creates <see
/// cref="IWorker"/> instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericWorkerFactory<T> : IWorkerFactory where T : IWorker
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Create an instance of <see cref="GenericWorkerFactory{T}"/>.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider"/> instance that
    /// should be used for service resolution.</param>
    public GenericWorkerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Create an <see cref="IWorker"/> instance of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The <see cref="IWorker"/> instance.</returns>
    public IWorker Create() => ActivatorUtilities.CreateInstance<T>(_provider);
}