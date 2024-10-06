using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="GenericWorkerServiceRegistrar{T}"/>.
/// </summary>
public class GenericWorkerServiceRegistrarTest
{
    /// <summary>
    /// <see cref="GenericWorkerServiceRegistrar{T}.RegisterWorkers"/> should
    /// throw if the given service collection argument is null.
    /// </summary>
    [Fact]
    public void RegisterWorkers_ThrowsWhenTheGivenServiceCollectionIsNull()
    {
        TestWorkerServiceRegistrar subject = new();
        Scenario.ThrowsWhenArgumentIsNull(
            () => subject.RegisterWorkers(null!, 5),
            "services"
        );
    }

    /// <summary>
    /// <see cref="GenericWorkerServiceRegistrar{T}.RegisterWorkers"/> should
    /// throw if the given count argument is less than one.
    /// </summary>
    [Fact]
    public void RegisterWorkers_ThrowsWhenTheGivenCountIsNegative()
    {
        TestWorkerServiceRegistrar subject = new();
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidWorkercount) => subject.RegisterWorkers(
                new ServiceCollection(),
                invalidWorkercount
            ),
            "count"
        );
    }

    /// <summary>
    /// <see cref="GenericWorkerServiceRegistrar{T}.RegisterWorkers"/> should
    /// register the expected number of workers.
    /// </summary>
    [Fact]
    public void RegisterWorkers_RegistersTheExpectedNumberOfWorkers()
    {
        TestWorkerServiceRegistrar subject = new();
        foreach (int count in Enumerable.Range(0, 5))
        {
            var mock = new Mock<IServiceCollection>();
            var serviceCollection = mock.Object;
            var returnValue = subject.RegisterWorkers(serviceCollection, count);
            Assert.Same(serviceCollection, returnValue);
            mock.Verify(
                _ => _.Add(It.IsAny<ServiceDescriptor>()),
                Times.Exactly(count)
            );
        }
    }

    internal class TestWorkerServiceRegistrar : GenericWorkerServiceRegistrar<TestWorkerService>
    {
    }

    internal class TestWorkerService : BackgroundService
    {
        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
