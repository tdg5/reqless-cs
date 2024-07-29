using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RetryJobAsync"/>.
/// </summary>
public class RetryJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given group name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfGroupNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidGroupName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: invalidGroupName!,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "groupName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given job ID is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: invalidJid!,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given message is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfMessageIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidMessage) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: invalidMessage!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "message"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given queue name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given delay is negative.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDelayIsNegative()
    {
        await Scenario.ThrowsWhenParameterIsNegativeAsync(
            (invalidDelay) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    delay: invalidDelay,
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "delay"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var delay = 0;

        bool retriedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RetryJobAsync(
                ExampleJid,
                ExampleQueueName,
                ExampleWorkerName,
                ExampleGroupName,
                ExampleMessage,
                delay
            ),
            expectedArguments: [
                "job.retry",
                0,
                ExampleJid,
                ExampleQueueName,
                ExampleWorkerName,
                delay,
                ExampleGroupName,
                ExampleMessage,
            ],
            returnValue: 1
        );
        Assert.True(retriedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if server returns
    /// null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var delay = 0;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    ExampleGroupName,
                    ExampleMessage,
                    delay
                ),
                expectedArguments: [
                    "job.retry",
                    0,
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    delay,
                    ExampleGroupName,
                    ExampleMessage,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }
}
