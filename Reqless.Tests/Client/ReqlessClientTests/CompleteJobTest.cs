using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CompleteJobAsync"/>.
/// </summary>
public class CompleteJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if data is
    /// null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: invalidData!,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if jid is
    /// null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: invalidJid!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if queue name
    /// is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if worker name
    /// is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        bool completedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CompleteJobAsync(
                data: ExampleData,
                jid: ExampleJid,
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName),
            expectedArguments: [
                "job.complete",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleQueueName,
                ExampleData,
            ],
            returnValue: "complete");
        Assert.True(completedSuccessfully);
    }
}
