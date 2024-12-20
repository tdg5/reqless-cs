using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfGroupNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidGroupName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: invalidGroupName!,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName!,
                    workerName: ExampleWorkerName)),
            "groupName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given job ID is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: invalidJid!,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given message is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfMessageIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidMessage) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: invalidMessage!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "message");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given queue name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if the
    /// given delay is negative.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDelayIsNegative()
    {
        await Scenario.ThrowsWhenArgumentIsNegativeAsync(
            (invalidDelay) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    delay: invalidDelay,
                    groupName: ExampleGroupName,
                    jid: ExampleJid,
                    message: ExampleMessage,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "delay");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
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
                delay),
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
            returnValue: 1);
        Assert.True(retriedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if server returns
    /// null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var delay = 0;

        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    ExampleGroupName,
                    ExampleMessage,
                    delay),
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
                returnValue: null));
    }
}
