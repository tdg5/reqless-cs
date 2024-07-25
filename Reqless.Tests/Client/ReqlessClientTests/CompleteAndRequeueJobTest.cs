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
    /// data is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: null!,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'data')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// data is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteAndRequeueJobAsync(
                        data: emptyString,
                        jid: ExampleJid,
                        nextQueueName: ExampleQueueName,
                        queueName: ExampleQueueName,
                        workerName: ExampleWorkerName
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'data')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: null!,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// jid is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteAndRequeueJobAsync(
                        data: ExampleData,
                        jid: emptyString,
                        nextQueueName: ExampleQueueName,
                        queueName: ExampleQueueName,
                        workerName: ExampleWorkerName
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// queue name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: null!,
                    workerName: ExampleWorkerName
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// queue name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteAndRequeueJobAsync(
                        data: ExampleData,
                        jid: ExampleJid,
                        nextQueueName: ExampleQueueName,
                        queueName: emptyString,
                        workerName: ExampleWorkerName
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// worker name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteAndRequeueJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    nextQueueName: ExampleQueueName,
                    queueName: ExampleQueueName,
                    workerName: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'workerName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should throw if
    /// worker name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteAndRequeueJobAsync(
                        data: ExampleData,
                        jid: ExampleJid,
                        nextQueueName: ExampleQueueName,
                        queueName: ExampleQueueName,
                        workerName: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'workerName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.CompleteAndRequeueJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
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