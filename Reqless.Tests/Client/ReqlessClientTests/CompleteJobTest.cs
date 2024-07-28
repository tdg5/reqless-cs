using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    [Fact]
    public async void ThrowsIfDataIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: invalidData!,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if jid is
    /// null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: invalidJid!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if queue name
    /// is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if worker name
    /// is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        bool completedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CompleteJobAsync(
                data: ExampleData,
                jid: ExampleJid,
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.complete",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleQueueName,
                ExampleData,
            ],
            returnValue: "complete"
        );
        Assert.True(completedSuccessfully);
    }
}