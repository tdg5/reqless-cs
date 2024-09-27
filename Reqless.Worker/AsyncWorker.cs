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
    /// An <see cref="IReqlessClient"/> instance to use for accessing Reqless.
    /// </summary>
    protected readonly IReqlessClient _reqlessClient;

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
    /// An <see cref="IJobReserver"/> instance to use for reserving jobs.
    /// </summary>
    protected readonly IJobReserver _jobReserver;

    /// <summary>
    /// Create an instance of <see cref="AsyncWorker"/>.
    /// </summary>
    /// <param name="jobReserver">An <see cref="IJobReserver"/> instance to use
    /// for reserving jobs.</param>
    /// <param name="reqlessClient">An <see cref="IReqlessClient"/> instance to
    /// use for accessing Reqless.</param>
    /// <param name="unitOfWorkActivator">An <see cref="IUnitOfWorkActivator"/>
    /// instance to use for creating unit of work instances.</param>
    /// <param name="unitOfWorkResolver">An <see cref="IUnitOfWorkResolver"/>
    /// instance to use for resolving unit of work types.</param>
    public AsyncWorker(
        IJobReserver jobReserver,
        IReqlessClient reqlessClient,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver
    )
    {
        _jobReserver = jobReserver;
        _reqlessClient = reqlessClient;
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

            try
            {
                // How are queue identifiers supposed to reach this point?
                // Should they be encapsulated somewhere else already?
                Job? job = await _jobReserver.TryReserveJobAsync();
                if (true || job is not null)
                {
                    await ExecuteJobAsync(
                        job!,
                        cancellationToken
                    );
                }
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
