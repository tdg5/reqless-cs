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
    /// Initializes a new instance of the <see cref="DefaultJobExecutor"/> class.
    /// </summary>
    /// <param name="jobContextFactory">An <see cref="IJobContextFactory"/> instance
    /// to use for making <see cref="IJobContext"/> instances.</param>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance that
    /// is used to create a new scope when creating unit of work instances.</param>
    /// <param name="unitOfWorkActivator">An <see cref="IUnitOfWorkActivator"/> instance
    /// to use for unit of work instantiation.</param>
    /// <param name="unitOfWorkResolver">An <see cref="IUnitOfWorkResolver"/> instance
    /// to use for unit of work resolution.</param>
    public DefaultJobExecutor(
        IJobContextFactory jobContextFactory,
        IServiceProvider serviceProvider,
        IUnitOfWorkActivator unitOfWorkActivator,
        IUnitOfWorkResolver unitOfWorkResolver)
    {
        ArgumentNullException.ThrowIfNull(jobContextFactory, nameof(jobContextFactory));
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(unitOfWorkActivator, nameof(unitOfWorkActivator));
        ArgumentNullException.ThrowIfNull(unitOfWorkResolver, nameof(unitOfWorkResolver));

        JobContextFactory = jobContextFactory;
        ServiceProvider = serviceProvider;
        UnitOfWorkActivator = unitOfWorkActivator;
        UnitOfWorkResolver = unitOfWorkResolver;
    }

    /// <summary>
    /// Gets the <see cref="IJobContextFactory"/> instance to use for making
    /// <see cref="IJobContext"/> instances.
    /// </summary>
    protected IJobContextFactory JobContextFactory { get; }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance that is used to create
    /// a new scope when creating unit of work instances.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the <see cref="IUnitOfWorkActivator"/> instance to use for unit of
    /// work instantiation.
    /// </summary>
    protected IUnitOfWorkActivator UnitOfWorkActivator { get; }

    /// <summary>
    /// Gets the <see cref="IUnitOfWorkResolver"/> instance to use for unit of
    /// work resolution.
    /// </summary>
    protected IUnitOfWorkResolver UnitOfWorkResolver { get; }

    /// <inheritdoc/>
    public async Task ExecuteAsync(Job job, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));

        Type unitOfWorkClass = UnitOfWorkResolver.Resolve(job.ClassName) ??
            throw new InvalidOperationException(
                $"Could not resolve {nameof(IUnitOfWork)} type '{job.ClassName}'.");

        using var scope = ServiceProvider.CreateAsyncScope();

        IJobContext jobContext =
            JobContextFactory.Create(scope.ServiceProvider, job);

        if (
            scope.ServiceProvider.GetService<IJobContextAccessor>()
                is IJobContextAccessor jobContextAccessor)
        {
            jobContextAccessor.Value = jobContext;
        }

        IUnitOfWork unitOfWork = UnitOfWorkActivator.CreateInstance(
            scope.ServiceProvider, unitOfWorkClass);

        await unitOfWork.PerformAsync(cancellationToken);
    }
}
