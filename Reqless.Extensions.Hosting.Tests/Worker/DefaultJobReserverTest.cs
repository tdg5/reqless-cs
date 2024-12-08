using Moq;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for <see cref="DefaultJobReserver"/>.
/// </summary>
public class DefaultJobReserverTest
{
    /// <summary>
    /// <see cref="DefaultJobReserver(IQueueNameProvider, IReqlessClient)"/>
    /// should throw when the given queue name provider is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenQueueNameProviderIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultJobReserver(null!, Mock.Of<IReqlessClient>()),
            "queueNameProvider");
    }

    /// <summary>
    /// <see cref="DefaultJobReserver(IQueueNameProvider, IReqlessClient)"/>
    /// should throw when the given reqless client is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenReqlessClientIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DefaultJobReserver(Mock.Of<IQueueNameProvider>(), null!),
            "reqlessClient");
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// should throw when the given worker name is null or empty or white space.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_ThrowsWhenWorkerNameIsNullOrEmptyOrWhiteSpace()
    {
        DefaultJobReserver subject =
            new(Mock.Of<IQueueNameProvider>(), Mock.Of<IReqlessClient>());
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidValue) => subject.TryReserveJobAsync(invalidValue!),
            "workerName");
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// should return null when no queue names are resolved.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_ReturnsNullWhenNoQueueNamesAreResolved()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        var queueNameProviderMock = mockRepository.Create<IQueueNameProvider>();
        queueNameProviderMock
            .Setup(_ => _.GetQueueNamesAsync())
            .ReturnsAsync([])
            .Verifiable();
        DefaultJobReserver subject = new(
            queueNameProvider: queueNameProviderMock.Object,
            reqlessClient: mockRepository.Create<IReqlessClient>().Object);
        var job = await subject.TryReserveJobAsync("workerName");
        Assert.Null(job);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// should return null when cancellation is requested.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_ReturnsNullWhenCancellationIsRequested()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        var queueNameProviderMock = mockRepository.Create<IQueueNameProvider>();
        queueNameProviderMock
            .Setup(_ => _.GetQueueNamesAsync())
            .ReturnsAsync(["a"])
            .Verifiable();
        DefaultJobReserver subject = new(
            queueNameProvider: queueNameProviderMock.Object,
            reqlessClient: mockRepository.Create<IReqlessClient>().Object);
        var cancellationToken = new CancellationToken(canceled: true);
        var job = await subject.TryReserveJobAsync("workerName", cancellationToken);
        Assert.Null(job);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// should check for cancellation before attempting to pop a job.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_ChecksForCancellationBeforeAttemptingToPopJob()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        var queueNameProviderMock = mockRepository.Create<IQueueNameProvider>();
        queueNameProviderMock
            .Setup(_ => _.GetQueueNamesAsync())
            .ReturnsAsync(["a", "b"])
            .Verifiable();
        var reqlessClientMock = mockRepository.Create<IReqlessClient>();
        CancellationTokenSource cancellationTokenSource = new();
        var cancellationToken = cancellationTokenSource.Token;
        reqlessClientMock
            .Setup(_ => _.PopJobAsync("a", "workerName"))
            .ReturnsAsync((Job?)null)
            .Callback(cancellationTokenSource.Cancel)
            .Verifiable();
        DefaultJobReserver subject = new(
            queueNameProvider: queueNameProviderMock.Object,
            reqlessClient: reqlessClientMock.Object);
        var job = await subject.TryReserveJobAsync("workerName", cancellationToken);
        Assert.Null(job);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// attempts to pop a job from each queue in order until a job is found.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_ChecksAllQueuesForAJobUntilAJobIsFound()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        List<string> queueNames = ["a", "b", "c"];
        var queueNameProviderMock = mockRepository.Create<IQueueNameProvider>();
        queueNameProviderMock
            .Setup(_ => _.GetQueueNamesAsync())
            .ReturnsAsync(queueNames)
            .Verifiable();
        var reqlessClientMock = mockRepository.Create<IReqlessClient>();
        foreach (var queueName in queueNames)
        {
            reqlessClientMock
                .Setup(_ => _.PopJobAsync(queueName, "workerName"))
                .ReturnsAsync((Job?)null)
                .Verifiable();
        }

        DefaultJobReserver subject = new(
            queueNameProvider: queueNameProviderMock.Object,
            reqlessClient: reqlessClientMock.Object);
        var job = await subject.TryReserveJobAsync("workerName");
        Assert.Null(job);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="DefaultJobReserver.TryReserveJobAsync(string, CancellationToken?)"/>
    /// does not check subsequent queues after a job is found and returned.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task TryReserveJobAsync_DoesNotCheckSubsequentQueuesAfterJobIsFound()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        List<string> queueNames = ["a", "b", "c"];
        var queueNameProviderMock = mockRepository.Create<IQueueNameProvider>();
        queueNameProviderMock
            .Setup(_ => _.GetQueueNamesAsync())
            .ReturnsAsync(queueNames)
            .Verifiable();
        var expectedJob = JobFactory.NewJob();
        var reqlessClientMock = mockRepository.Create<IReqlessClient>();
        reqlessClientMock
            .Setup(_ => _.PopJobAsync(queueNames[0], "workerName"))
            .ReturnsAsync(expectedJob)
            .Verifiable();
        DefaultJobReserver subject = new(
            queueNameProvider: queueNameProviderMock.Object,
            reqlessClient: reqlessClientMock.Object);
        var job = await subject.TryReserveJobAsync("workerName");
        Assert.Equal(expectedJob, job);
        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }
}
