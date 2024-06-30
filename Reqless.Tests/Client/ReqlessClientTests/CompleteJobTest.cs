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
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: null!,
                    jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if data is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteJobAsync(
                        data: emptyString,
                        jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if jid is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: null!,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if jid is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteJobAsync(
                        data: ExampleData,
                        jid: emptyString,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if queue name
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if queue name
    /// is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteJobAsync(
                        data: ExampleData,
                        jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if worker name
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should throw if worker name
    /// is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CompleteJobAsync(
                        data: ExampleData,
                        jid: ExampleJid,
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
    /// <see cref="ReqlessClient.CompleteJobAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                bool completedSuccessfully = await subject.CompleteJobAsync(
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                );
                Assert.True(completedSuccessfully);
            },
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
    }
}
