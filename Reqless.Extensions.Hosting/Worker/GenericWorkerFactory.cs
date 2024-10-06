using Microsoft.Extensions.DependencyInjection;

namespace Reqless.Extensions.Hosting.Worker;

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
        ArgumentNullException.ThrowIfNull(
            workerNameProvider,
            nameof(workerNameProvider)
        );

        _workerNameProvider = workerNameProvider;
    }

    /// <summary>
    /// Create an <see cref="IWorker"/> instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance
    /// that can be utilized when creating worker instances.</param>
    /// <returns>The <see cref="IWorker"/> instance.</returns>
    public IWorker Create(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

        var workerName = _workerNameProvider.GetWorkerName();
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, workerName);
    }
}
