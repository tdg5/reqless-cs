using Microsoft.Extensions.DependencyInjection;
using Reqless.Client.Models;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// Default implementation of <see cref="IJobExecutor"/>.
/// </summary>
public class DefaultJobExecutor : IJobExecutor
{
    /// <summary>
    /// An <see cref="IJobContextFactory"/> instance to use for making <see
    /// cref="IJobContext"/> instances.
    /// </summary>
    protected readonly IJobContextFactory _jobContextFactory;

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
    /// Create an instance of <see cref="DefaultJobExecutor"/>.
    /// </summary>
    public DefaultJobExecutor(
        IJobContextFactory jobContextFactory,
        IServiceProvider serviceProvider,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver
    )
    {
        _jobContextFactory = jobContextFactory;
        _serviceProvider = serviceProvider;
        _unitOfWorkActivator = unitOfWorkActivator;
        _unitOfWorkResolver = unitOfWorkResolver;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(Job job, CancellationToken cancellationToken)
    {
        Type unitOfWorkClass = _unitOfWorkResolver.Resolve(job.ClassName) ??
            throw new InvalidOperationException(
                $"Could not resolve {nameof(IUnitOfWork)} type '{job.ClassName}'."
            );

        ExecutionContext? initialExecutionContext = ExecutionContext.Capture();
        IJobContext? jobContext = _jobContextFactory.Create(job);
        try
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            IUnitOfWork unitOfWork = _unitOfWorkActivator.CreateInstance(
                scope.ServiceProvider,
                unitOfWorkClass
            );
            await unitOfWork.PerformAsync(cancellationToken);
        }
        finally
        {
            if (jobContext is not null)
            {
                _jobContextFactory.DisposeContext(jobContext);
            }

            if (initialExecutionContext is not null)
            {
                // Clear any AsyncLocals set during job execution back to a
                // clean state ready for next job.
                ExecutionContext.Restore(initialExecutionContext);
            }
        }
    }
}
