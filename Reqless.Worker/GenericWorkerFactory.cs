using Microsoft.Extensions.DependencyInjection;
using Reqless.Extensions.Hosting.Worker;

namespace Reqless.Worker;

/// <summary>
/// Implementation of <see cref="IWorkerFactory"/> that creates <see
/// cref="IWorker"/> instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericWorkerFactory<T> : IWorkerFactory where T : IWorker
{
    private readonly IWorkerNameProvider _workerNameProvider;

    /// <summary>
    /// Create an instance of <see cref="GenericWorkerFactory{T}"/>.
    /// </summary>
    /// <param name="workerNameProvider">The <see cref="IWorkerNameProvider"/>
    /// instance that should be used to get a name of the worker.</param>
    public GenericWorkerFactory(IWorkerNameProvider workerNameProvider)
    {
        _workerNameProvider = workerNameProvider;
    }

    /// <summary>
    /// Create an <see cref="IWorker"/> instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance
    /// that can be utilized when creating worker instances.</param>
    /// <returns>The <see cref="IWorker"/> instance.</returns>
    public IWorker Create(IServiceProvider serviceProvider) =>
        ActivatorUtilities.CreateInstance<T>(
            serviceProvider,
            _workerNameProvider.GetWorkerName()
        );
}
