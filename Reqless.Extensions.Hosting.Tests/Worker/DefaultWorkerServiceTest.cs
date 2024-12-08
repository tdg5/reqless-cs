using Moq;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultWorkerService"/>.
/// </summary>
public class DefaultWorkerServiceTest
{
    /// <summary>
    /// <see cref="DefaultWorkerService(IServiceProvider, IWorkerFactory)"/>
    /// throws when the given service provider is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenServiceProviderIsNull()
    {
        IWorkerFactory workerFactory = Mock.Of<IWorkerFactory>();
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultWorkerService(null!, workerFactory),
            "serviceProvider");
    }

    /// <summary>
    /// <see cref="DefaultWorkerService(IServiceProvider, IWorkerFactory)"/>
    /// throws when the given worker factory is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenWorkerFactoryIsNull()
    {
        IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultWorkerService(serviceProvider, null!),
            "workerFactory");
    }

    /// <summary>
    /// <see cref="DefaultWorkerService.ExecuteAsync(CancellationToken)"/>
    /// creates a worker and calls <see
    /// cref="IWorker.ExecuteAsync(CancellationToken)"/>.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_CreatesWorkerAndCallsExecuteAsync()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        var serviceProvider = mockRepository.Create<IServiceProvider>().Object;
        var workerMock = mockRepository.Create<IWorker>();
        workerMock
            .Setup(_ => _.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var factoryMock = new Mock<IWorkerFactory>();
        factoryMock
            .Setup(_ => _.Create(serviceProvider))
            .Returns(workerMock.Object)
            .Verifiable();
        DefaultWorkerService subject = new(serviceProvider, factoryMock.Object);
        CancellationToken cancellationToken = default;
        await subject.StartAsync(cancellationToken);
        await (subject.ExecuteTask ?? Task.CompletedTask);
        await subject.StopAsync(cancellationToken);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }
}
