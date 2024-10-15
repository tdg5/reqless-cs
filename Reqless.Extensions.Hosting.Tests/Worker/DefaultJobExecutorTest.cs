using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqless.Common.Utilities;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using Reqless.Tests.Fixtures.UnitOfWork;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Test class for <see cref="DefaultJobExecutor"/>.
/// </summary>
public class DefaultJobExecutorTest
{
    /// <summary>
    /// <see cref="DefaultJobExecutor(IJobContextFactory, IServiceProvider,
    /// IUnitOfWorkActivator, IUnitOfWorkResolver)"/> throws when the given job
    /// context factory is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenJobContextFactoryIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(
                jobContextFactory: Maybe<IJobContextFactory>.None
            ),
            "jobContextFactory"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor(IJobContextFactory, IServiceProvider,
    /// IUnitOfWorkActivator, IUnitOfWorkResolver)"/> throws when the given
    /// service provider is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenServiceProviderIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(
                serviceProvider: Maybe<ServiceProvider>.None
            ),
            "serviceProvider"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor(IJobContextFactory, IServiceProvider,
    /// IUnitOfWorkActivator, IUnitOfWorkResolver)"/> throws when the given
    /// unit of work activator is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenUnitOfWorkActivatorIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(
                unitOfWorkActivator: Maybe<IUnitOfWorkActivator>.None
            ),
            "unitOfWorkActivator"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor(IJobContextFactory, IServiceProvider,
    /// IUnitOfWorkActivator, IUnitOfWorkResolver)"/> throws when the given
    /// unit of work resolver is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenUnitOfWorkResolverIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(
                unitOfWorkResolver: Maybe<IUnitOfWorkResolver>.None
            ),
            "unitOfWorkResolver"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor.ExecuteAsync"/> throws when the given job
    /// is null.
    /// </summary>
    [Fact]
    public async void ExecuteAsync_ThrowsWhenTheGivenJobIsNull()
    {
        var subject = MakeSubject();
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => subject.ExecuteAsync(null!, CancellationToken.None),
            "job"
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor.ExecuteAsync"/> throws when the unit of
    /// work resolver is unable to resolve the job class.
    /// </summary>
    [Fact]
    public async void ExecuteAsync_ThrowsWhenUnitOfWorkResolverReturnsNull()
    {
        var expectedJobClassName = "Some.Job.Class";
        var mockRepository = new MockRepository(MockBehavior.Strict);
        var unitOfWorkResolverMock = mockRepository.Create<IUnitOfWorkResolver>();
        unitOfWorkResolverMock
            .Setup(_ => _.Resolve(expectedJobClassName))
            .Returns((Type?)null);

        var job = JobFactory.NewJob(className: Maybe.Some(expectedJobClassName));
        var subject = MakeSubject(
            unitOfWorkResolver: Maybe.Some(unitOfWorkResolverMock.Object)
        );
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => subject.ExecuteAsync(job, CancellationToken.None)
        );
        Assert.Equal(
            $"Could not resolve IUnitOfWork type '{expectedJobClassName}'.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="DefaultJobExecutor.ExecuteAsync"/> performs the unit of work
    /// with the expected supporting environment.
    /// </summary>
    [Fact]
    public async void ExecuteAsync_PerformsTheUnitOfWorkWithTheExpectedEnvironment()
    {
        bool actionExecuted = false;
        ServiceProvider? rootServiceProvider = null;
        DelegateUnitOfWorkAction? expectedDelegateUnitOfWorkAction = null;
        CancellationTokenSource cancellationTokenSource = new();

        var expectedCancellationToken = cancellationTokenSource.Token;
        var expectedJob = JobFactory.NewJob(
            className: Maybe.Some(typeof(DelegateUnitOfWork).FullName!)
        );

        Task Action(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            // A scoped service provider is expected, so the service provider
            // instance should be different.
            Assert.NotNull(rootServiceProvider);
            Assert.NotEqual(rootServiceProvider, serviceProvider);

            // Ensure that we are able to resolve a scoped service to the expected instance.
            var delegateUnitOfWorkAction =
                serviceProvider.GetRequiredService<DelegateUnitOfWorkAction>();
            Assert.NotNull(delegateUnitOfWorkAction);
            Assert.Equal(expectedDelegateUnitOfWorkAction, delegateUnitOfWorkAction);

            // The same cancellation token is expected.
            Assert.Equal(expectedCancellationToken, cancellationToken);

            // It is expected that a job context was created and made available to
            // the job context accessor.
            var jobContextAccessor = serviceProvider.GetRequiredService<IJobContextAccessor>();
            var jobContext = jobContextAccessor.Value;
            Assert.NotNull(jobContext);
            Assert.Equal(expectedJob, jobContext.Job);

            actionExecuted = true;
            return Task.CompletedTask;
        }

        ServiceCollection services = new();
        services.AddScoped(serviceProvider =>
        {
            expectedDelegateUnitOfWorkAction = new DelegateUnitOfWorkAction(Action);
            return expectedDelegateUnitOfWorkAction;
        });
        services.AddSingleton<IJobContextAccessor, DefaultJobContextAccessor>();
        rootServiceProvider = services.BuildServiceProvider();
        var subject = MakeSubject(
            serviceProvider: Maybe.Some(rootServiceProvider)
        );
        await subject.ExecuteAsync(expectedJob, expectedCancellationToken);
        Assert.True(actionExecuted);
    }

    private static DefaultJobExecutor MakeSubject(
        Maybe<IJobContextFactory>? jobContextFactory = null,
        Maybe<ServiceProvider>? serviceProvider = null,
        Maybe<IUnitOfWorkActivator>? unitOfWorkActivator = null,
        Maybe<IUnitOfWorkResolver>? unitOfWorkResolver = null
    )
    {
        Maybe<IJobContextFactory> _jobContextFactory = jobContextFactory
            ?? Maybe<IJobContextFactory>.Some(new DefaultJobContextFactory());
        Maybe<ServiceProvider> _serviceProvider = serviceProvider
            ?? Maybe<ServiceProvider>.Some(
                new ServiceCollection().BuildServiceProvider()
            );
        Maybe<IUnitOfWorkActivator> _unitOfWorkActivator = unitOfWorkActivator
            ?? Maybe<IUnitOfWorkActivator>.Some(new DefaultUnitOfWorkActivator());
        Maybe<IUnitOfWorkResolver> _unitOfWorkResolver = unitOfWorkResolver
            ?? Maybe<IUnitOfWorkResolver>.Some(new DefaultUnitOfWorkResolver());

        return new(
            jobContextFactory: _jobContextFactory.GetOrDefault(null!),
            serviceProvider: _serviceProvider.GetOrDefault(null!),
            unitOfWorkActivator: _unitOfWorkActivator.GetOrDefault(null!),
            unitOfWorkResolver: _unitOfWorkResolver.GetOrDefault(null!)
        );
    }
}
