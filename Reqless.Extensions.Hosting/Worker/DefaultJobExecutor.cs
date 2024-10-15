using Microsoft.Extensions.DependencyInjection;
using Reqless.Client.Models;
using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

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
        ArgumentNullException.ThrowIfNull(jobContextFactory, nameof(jobContextFactory));
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(unitOfWorkActivator, nameof(unitOfWorkActivator));
        ArgumentNullException.ThrowIfNull(unitOfWorkResolver, nameof(unitOfWorkResolver));

        _jobContextFactory = jobContextFactory;
        _serviceProvider = serviceProvider;
        _unitOfWorkActivator = unitOfWorkActivator;
        _unitOfWorkResolver = unitOfWorkResolver;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(Job job, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));

        Type unitOfWorkClass = _unitOfWorkResolver.Resolve(job.ClassName) ??
            throw new InvalidOperationException(
                $"Could not resolve {nameof(IUnitOfWork)} type '{job.ClassName}'."
            );

        using var scope = _serviceProvider.CreateAsyncScope();

        IJobContext jobContext = _jobContextFactory.Create(scope.ServiceProvider, job);

        if (
            scope.ServiceProvider.GetService<IJobContextAccessor>()
                is IJobContextAccessor jobContextAccessor
        )
        {
            jobContextAccessor.Value = jobContext;
        }

        IUnitOfWork unitOfWork = _unitOfWorkActivator.CreateInstance(
            scope.ServiceProvider,
            unitOfWorkClass
        );

        await unitOfWork.PerformAsync(cancellationToken);
    }
}
