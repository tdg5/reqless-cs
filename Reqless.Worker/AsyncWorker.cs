using Reqless.Framework;
using Reqless.Client;
using Reqless.Client.Models;

namespace Reqless.Worker;

/// <summary>
/// <see cref="IWorker"/> that performs jobs serially in an async context.
/// </summary>
public class AsyncWorker : IWorker
{
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
    /// An <see cref="IUnitOfWorkActivator"/> instance to use for unit of work
    /// instantiation.
    /// </summary>
    protected readonly IUnitOfWorkActivator _unitOfWorkActivator;

    /// <summary>
    /// An <see cref="IUnitOfWorkResolver"/> instance to use for unit of work
    /// resolution.
    /// </summary>
    protected readonly IUnitOfWorkResolver _unitOfWorkResolver;

    /// <summary>
    /// Create an instance of <see cref="AsyncWorker"/>.
    /// </summary>
    /// <param name="reqlessClientAccessor">An <see
    /// cref="IReqlessClientAccessor"/> instance to use for accessing the
    /// Reqless client.</param>
    /// <param name="reqlessClientFactory">An <see cref="IReqlessClientFactory"/> instance
    /// to use for creating Reqless clients.</param>
    /// <param name="unitOfWorkActivator">An <see
    /// cref="IUnitOfWorkActivator"/> instance to use for creating unit of work
    /// instances.</param>
    /// <param name="unitOfWorkResolver">An <see cref="IUnitOfWorkResolver"/>
    /// instance to use for resolving unit of work types.</param>
    public AsyncWorker(
        IReqlessClientAccessor reqlessClientAccessor,
        IReqlessClientFactory reqlessClientFactory,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver
    )
    {
        _reqlessClientAccessor = reqlessClientAccessor;
        _reqlessClientFactory = reqlessClientFactory;
        _unitOfWorkActivator = unitOfWorkActivator;
        _unitOfWorkResolver = unitOfWorkResolver;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
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

            IReqlessClient client = _reqlessClientFactory.Create();
            _reqlessClientAccessor.Value = client;
            try
            {
                Job? job = null;
                await ExecuteJobAsync(
                    null!,
                    cancellationToken
                );
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
    /// <param name="job">The job to execute.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// indicating whether or not work on the job should be discontinued prior
    /// to completion.</param>
    /// <returns>A <see cref="Task"/> instance that communicates when work has
    /// completed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the unit of work
    /// class can't be resolved or instantiated.</exception>
    public virtual Task ExecuteJobAsync(
        Job job,
        CancellationToken cancellationToken
    )
    {
        var unitOfWorkClassName = "Reqless.ExampleWorkerApp.ConcreteUnitOfWork";
        Type unitOfWorkClass = _unitOfWorkResolver.Resolve(unitOfWorkClassName) ??
            throw new InvalidOperationException(
                $"Could not resolve {nameof(IUnitOfWork)} type '{unitOfWorkClassName}'."
            );

        IUnitOfWork unitOfWork = _unitOfWorkActivator.CreateInstance(unitOfWorkClass);
        return unitOfWork.PerformAsync(cancellationToken);
    }
}
