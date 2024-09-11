using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Reqless.Framework;
using System.Threading;
using Reqless.Client;

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
    /// An <see cref="IReqlessClientAccessor"/> instance to use for accessing
    /// the Reqless client.
    /// </summary>
    protected readonly IReqlessClientAccessor _reqlessClientAccessor;

    /// <summary>
    /// An <see cref="IReqlessClientFactory"/> instance to use for creating Reqless clients.
    /// </summary>
    protected readonly IReqlessClientFactory _reqlessClientFactory;

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
    /// <param name="unitOfWorkActivator">An <see
    /// cref="IUnitOfWorkActivator"/> instance to use for creating unit of work
    /// instances.</param>
    /// <param name="unitOfWorkResolver">An <see cref="IUnitOfWorkResolver"/>
    /// instance to use for resolving unit of work types.</param>
    /// <param name="reqlessClientAccessor">An <see
    /// cref="IReqlessClientAccessor"/> instance to use for accessing the
    /// Reqless client.</param>
    /// <param name="reqlessClientFactory">An <see cref="IReqlessClientFactory"/> instance
    /// to use for creating Reqless clients.</param>
    public SerialWorker(
        IServiceProvider provider,
        IReqlessClientAccessor reqlessClientAccessor,
        IReqlessClientFactory reqlessClientFactory,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver
    )
    {
        _provider = provider;
        _reqlessClientAccessor = reqlessClientAccessor;
        _reqlessClientFactory = reqlessClientFactory;
        _unitOfWorkActivator = unitOfWorkActivator;
        _unitOfWorkResolver = unitOfWorkResolver;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        ExecutionContext? initialExecutionContext = ExecutionContext.Capture();
        while (!cancellationToken.IsCancellationRequested)
        {
            if (initialExecutionContext is not null)
            {
                // Clear any AsyncLocals set during job execution back to a
                // clean state ready for next job.
                ExecutionContext.Restore(initialExecutionContext);
            }

            IClient client = _reqlessClientFactory.Create();
            _reqlessClientAccessor.Value = client;
            try
            {
                await ExecuteJobAsync(cancellationToken);
            }
            finally
            {
                Reset();
            }
            await Task.Delay(1_000, cancellationToken);
        }
    }

    private void Reset()
    {
        _reqlessClientAccessor.Value = null;
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
