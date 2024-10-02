using Microsoft.Extensions.DependencyInjection;
using Reqless.Client.Models;
using Reqless.Client;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// <see cref="IWorker"/> that performs jobs serially in an async context.
/// </summary>
public class AsyncWorker : IWorker
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// An <see cref="IJobContextFactory"/> instance to use for making <see
    /// cref="IJobContext"/> instances.
    /// </summary>
    protected readonly IJobContextFactory _jobContextFactory;

    /// <summary>
    /// An <see cref="IReqlessClient"/> instance to use for accessing Reqless.
    /// </summary>
    protected readonly IReqlessClient _reqlessClient;

    /// <summary>
    /// A <see cref="IServiceProvider"/> instance that is used to create a new
    /// scope when creating unit of work instances.
    /// </summary>
    protected readonly IServiceProvider _serviceProvider;

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
    /// <param name="jobContextFactory">An <see cref="IJobContextFactory"/>
    /// instance to use for making <see cref="IJobContext"/> instances.</param>
    /// <param name="jobReserver">An <see cref="IJobReserver"/> instance to use
    /// for reserving jobs.</param>
    /// <param name="name">The name the worker should use when communicating
    /// with Reqless.</param>
    /// <param name="reqlessClient">An <see cref="IReqlessClient"/> instance to
    /// use for accessing Reqless.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance
    /// that is used to create a new scope when creating unit of work
    /// instances.</param>
    /// <param name="unitOfWorkActivator">An <see cref="IUnitOfWorkActivator"/>
    /// instance to use for creating unit of work instances.</param>
    /// <param name="unitOfWorkResolver">An <see cref="IUnitOfWorkResolver"/>
    /// instance to use for resolving unit of work types.</param>
    public AsyncWorker(
        IJobContextFactory jobContextFactory,
        IJobReserver jobReserver,
        string name,
        IReqlessClient reqlessClient,
        IServiceProvider serviceProvider,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver
    )
    {
        _jobContextFactory = jobContextFactory;
        _jobReserver = jobReserver;
        _reqlessClient = reqlessClient;
        _serviceProvider = serviceProvider;
        _unitOfWorkActivator = unitOfWorkActivator;
        _unitOfWorkResolver = unitOfWorkResolver;
        Name = name;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        ExecutionContext? initialExecutionContext = ExecutionContext.Capture();
        IJobContext? jobContext = null;
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
                Job? job = await _jobReserver.TryReserveJobAsync(Name);
                if (job is not null)
                {
                    jobContext = _jobContextFactory.Create(job);
                    await ExecuteJobAsync(
                        job,
                        cancellationToken
                    );
                }
            }
            finally
            {
                if (jobContext is not null)
                {
                    _jobContextFactory.DisposeContext(jobContext);
                }
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
        Type unitOfWorkClass = _unitOfWorkResolver.Resolve(job.ClassName) ??
            throw new InvalidOperationException(
                $"Could not resolve {nameof(IUnitOfWork)} type '{job.ClassName}'."
            );

        using var scope = _serviceProvider.CreateAsyncScope();
        IUnitOfWork unitOfWork = _unitOfWorkActivator.CreateInstance(
            scope.ServiceProvider,
            unitOfWorkClass
        );
        return unitOfWork.PerformAsync(cancellationToken);
    }
}
