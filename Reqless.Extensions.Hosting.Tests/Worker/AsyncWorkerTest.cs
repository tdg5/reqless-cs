using Microsoft.Extensions.Logging;
using Moq;
using Reqless.Client.Models;
using Reqless.Common.Utilities;
using Reqless.Extensions.Hosting.Worker;
using Reqless.Tests.Common.Client.Models;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Extensions.Hosting.Tests.Worker;

/// <summary>
/// Tests for the <see cref="AsyncWorker"/> class.
/// </summary>
public class AsyncWorkerTest
{
    /// <summary>
    /// <see cref="AsyncWorker(IJobExecutor, IJobReserver, ILogger{AsyncWorker}, string)"/>
    /// should throw when the given job executor is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenJobExecutorIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(jobExecutor: Maybe<IJobExecutor>.None),
            "jobExecutor");
    }

    /// <summary>
    /// <see cref="AsyncWorker(IJobExecutor, IJobReserver, ILogger{AsyncWorker}, string)"/>
    /// should throw when the given job reserver is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenJobReserverIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(jobReserver: Maybe<IJobReserver>.None),
            "jobReserver");
    }

    /// <summary>
    /// <see cref="AsyncWorker(IJobExecutor, IJobReserver, ILogger{AsyncWorker}, string)"/>
    /// should throw when the given logger is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenLoggerIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => MakeSubject(logger: Maybe<ILogger<AsyncWorker>>.None),
            "logger");
    }

    /// <summary>
    /// <see cref="AsyncWorker(IJobExecutor, IJobReserver, ILogger{AsyncWorker}, string)"/>
    /// should throw when the given name is null or empty or
    /// whitespace.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenGivenNameIsNullOrEmptyOrWhiteSpace()
    {
        Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespace(
            (invalidName) => MakeSubject(name: Maybe<string>.Some(invalidName!)),
            "name");
    }

    /// <summary>
    /// <see cref="AsyncWorker(IJobExecutor, IJobReserver, ILogger{AsyncWorker}, string)"/>
    /// should set the <see cref="AsyncWorker.Name"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsNameProperty()
    {
        var expectedName = "expected-worker-name";
        var subject = MakeSubject(name: Maybe.Some(expectedName));
        Assert.Equal(expectedName, subject.Name);
    }

    /// <summary>
    /// <see cref="AsyncWorker.ExecuteAsync"/> should not try to reserve or
    /// execute a job when cancellation is requested.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_DoesNotReserveOrExecuteWhenCancellationIsRequested()
    {
        var jobReserverMock = new Mock<IJobReserver>(MockBehavior.Strict);
        var subject = MakeSubject(jobReserver: Maybe.Some(jobReserverMock.Object));
        var cancellationToken = new CancellationToken(canceled: true);
        await subject.ExecuteAsync(cancellationToken);
        jobReserverMock.Verify();
        jobReserverMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="AsyncWorker.ExecuteAsync"/> should reserve and execute a job.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_ReservesAndExecutesJob()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);

        var expectedWorkerName = "expected-worker-name";
        var job = JobFactory.NewJob();
        CancellationTokenSource cancellationTokenSource = new();
        var jobReserverMock = mockRepository.Create<IJobReserver>();
        jobReserverMock
            .Setup(_ =>
                _.TryReserveJobAsync(
                    expectedWorkerName, cancellationTokenSource.Token))
            .Callback(cancellationTokenSource.Cancel)
            .ReturnsAsync(job)
            .Verifiable();

        var jobExecutorMock = mockRepository.Create<IJobExecutor>();
        jobExecutorMock
            .Setup(_ => _.ExecuteAsync(job, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var subject = MakeSubject(
            jobReserver: Maybe.Some(jobReserverMock.Object),
            jobExecutor: Maybe.Some(jobExecutorMock.Object),
            name: Maybe.Some(expectedWorkerName));
        await subject.ExecuteAsync(cancellationTokenSource.Token);

        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="AsyncWorker.ExecuteAsync"/> should log an error when an
    /// exception is thrown during job execution.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_LogsAnErrorWhenJobExecutionThrows()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);

        var expectedWorkerName = "expected-worker-name";
        var job = JobFactory.NewJob();
        CancellationTokenSource cancellationTokenSource = new();
        var jobReserverMock = mockRepository.Create<IJobReserver>();
        jobReserverMock
            .Setup(_ =>
                _.TryReserveJobAsync(
                    expectedWorkerName, cancellationTokenSource.Token))
            .ReturnsAsync(job)
            .Verifiable();

        var expectedException = new InvalidOperationException("boom");
        var jobExecutorMock = mockRepository.Create<IJobExecutor>();
        jobExecutorMock
            .Setup(_ => _.ExecuteAsync(job, It.IsAny<CancellationToken>()))
            .Callback(cancellationTokenSource.Cancel)
            .ThrowsAsync(expectedException)
            .Verifiable();

        var loggerMock = mockRepository.Create<ILogger<AsyncWorker>>();
        loggerMock.VerifyLogError(
            expectedException, "An error occurred while processing a job.");

        var subject = MakeSubject(
            jobReserver: Maybe.Some(jobReserverMock.Object),
            jobExecutor: Maybe.Some(jobExecutorMock.Object),
            logger: Maybe.Some(loggerMock.Object),
            name: Maybe.Some(expectedWorkerName));
        await subject.ExecuteAsync(cancellationTokenSource.Token);

        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="AsyncWorker.ExecuteAsync"/> should not execute a job when
    /// <see cref="IJobReserver.TryReserveJobAsync"/> returns null.
    /// </summary>
    /// <returns>A Task denoting the completion of the test.</returns>
    [Fact]
    public async Task ExecuteAsync_DoesNotExecuteJobWhenReserveJobReturnsNull()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);

        var expectedWorkerName = "expected-worker-name";
        var jobReserverMock = mockRepository.Create<IJobReserver>();
        CancellationTokenSource cancellationTokenSource = new();
        jobReserverMock
            .Setup(_ =>
                _.TryReserveJobAsync(
                    expectedWorkerName, cancellationTokenSource.Token))
            .Callback(cancellationTokenSource.Cancel)
            .ReturnsAsync((Job?)null)
            .Verifiable();

        var jobExecutorMock = mockRepository.Create<IJobExecutor>();
        jobExecutorMock.Verify(
            _ => _.ExecuteAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()),
            Times.Never());

        var subject = MakeSubject(
            jobReserver: Maybe.Some(jobReserverMock.Object),
            jobExecutor: Maybe.Some(jobExecutorMock.Object),
            name: Maybe.Some(expectedWorkerName));
        await subject.ExecuteAsync(cancellationTokenSource.Token);

        mockRepository.Verify();
        mockRepository.VerifyNoOtherCalls();
    }

    private static AsyncWorker MakeSubject(
        Maybe<IJobExecutor>? jobExecutor = null,
        Maybe<IJobReserver>? jobReserver = null,
        Maybe<ILogger<AsyncWorker>>? logger = null,
        Maybe<string>? name = null)
    {
        var jobExecutorOrDefault = jobExecutor ?? Maybe.Some(Mock.Of<IJobExecutor>());
        var jobReserverOrDefault = jobReserver ?? Maybe.Some(Mock.Of<IJobReserver>());
        var loggerOrDefault = logger ?? Maybe.Some(Mock.Of<ILogger<AsyncWorker>>());
        var typeNameOrDefault = name ?? Maybe.Some("worker-name");
        return new AsyncWorker(
            jobExecutor: jobExecutorOrDefault.GetOrDefault(null!),
            jobReserver: jobReserverOrDefault.GetOrDefault(null!),
            logger: loggerOrDefault.GetOrDefault(null!),
            name: typeNameOrDefault.GetOrDefault(null!));
    }
}
