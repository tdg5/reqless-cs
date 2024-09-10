using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// Worker background service that performs jobs in a serial fashion.
/// </summary>
public class SerialWorker : BackgroundService
{
    /// <summary>
    /// An <see cref="IServiceProvider"/> instance to use for service resolution.
    /// </summary>
    protected readonly IServiceProvider _provider;

    /// <summary>
    /// An <see cref="IUnitOfWorkResolver"/> instance to use for unit of work
    /// resolution.
    /// </summary>
    protected readonly IUnitOfWorkResolver _unitOfWorkResolver;

    /// <summary>
    /// An <see cref="IUnitOfWorkActivator"/> instance to use for unit of work
    /// instantiation.
    /// </summary>
    protected readonly IUnitOfWorkActivator _unitOfWorkActivator;

    /// <summary>
    /// Create an instance of <see cref="SerialWorker"/>.
    /// </summary>
    /// <param name="provider">An <see cref="IServiceProvider"/> instance to use
    /// for service resolution.</param>
    public SerialWorker(IServiceProvider provider)
    {
        _provider = provider;
        _unitOfWorkActivator = provider.GetRequiredService<IUnitOfWorkActivator>();
        _unitOfWorkResolver = provider.GetRequiredService<IUnitOfWorkResolver>();
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ExecuteJobAsync(cancellationToken);
            await Task.Delay(1_000, cancellationToken);
        }
    }

    /// <summary>
    /// Execute the job.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// indicating whether or not work on the job should be discontinued prior
    /// to completion.</param>
    /// <returns>A <see cref="Task"/> instance that communicates when work has
    /// completed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the unit of work
    /// class can't be resolved or instantiated.</exception>
    public virtual async Task ExecuteJobAsync(CancellationToken cancellationToken)
    {
        var unitOfWorkClassName = "Reqless.ExampleWorkerApp.ConcreteUnitOfWork";
        var unitOfWorkClass = _unitOfWorkResolver.Resolve(unitOfWorkClassName) ??
            throw new InvalidOperationException($"Could not resolve the unit of work type '{unitOfWorkClassName}'.");

        IUnitOfWork unitOfWork = _unitOfWorkActivator.CreateInstance(_provider, unitOfWorkClass);
        await unitOfWork.PerformAsync();
    }
}
