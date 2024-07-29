using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/>.
/// </summary>
public class CompleteAndRequeueJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// data is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: invalidData!,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// jid is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: invalidJid!,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// queue name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// worker name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var dependencies = new string[] { "dependency1", "dependency2" };
        var delay = 42;
        var nextQueueName = "nextQueueName";
        foreach (var includeDependencies in new bool[] { true, false })
        {
            var _dependencies = includeDependencies ? dependencies : null;
            bool completedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    dependencies: _dependencies,
                    delay: delay,
                    jid: ExampleJid,
                    nextQueueName: nextQueueName,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "job.completeAndRequeue",
                    0,
                    ExampleJid,
                    ExampleWorkerName,
                    ExampleQueueName,
                    ExampleData,
                    nextQueueName,
                    "delay",
                    delay,
                    "depends",
                    JsonSerializer.Serialize(_dependencies ?? []),
                ],
                returnValue: includeDependencies ? "depends" : "scheduled"
            );
            Assert.True(completedSuccessfully);
        }
    }
}
