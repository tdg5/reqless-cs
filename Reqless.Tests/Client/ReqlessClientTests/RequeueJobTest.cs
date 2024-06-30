using System.Reflection;
using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RequeueJobAsync"/>.
/// </summary>
public class RequeueJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if workerName
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if workerName
    /// is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RequeueJobAsync(
                        className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if queueName is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if queueName
    /// is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RequeueJobAsync(
                        className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if jid is empty
    /// or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RequeueJobAsync(
                        className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if className is
    /// empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfClassNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RequeueJobAsync(
                        className: emptyString,
                        data: ExampleData,
                        jid: ExampleJid,
                        queueName: ExampleQueueName,
                        workerName: ExampleWorkerName
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'className')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if data is
    /// empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RequeueJobAsync(
                        className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should call the executor
    /// with the expected arguments when optional arguments are omitted.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsWhenOptionalArgumentsAreOmitted()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RequeueJobAsync(
                className: ExampleClassName,
                data: ExampleData,
                jid: ExampleJid,
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.requeue",
                0,
                ExampleWorkerName,
                ExampleQueueName,
                ExampleJid,
                ExampleClassName,
                ExampleData,
                0,
                "priority",
                RedisValue.Null,
                "retries",
                RedisValue.Null,
                "depends",
                RedisValue.Null,
                "tags",
                RedisValue.Null,
                "throttles",
                RedisValue.Null,
            ],
            returnValue: ExampleJid
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should call the executor
    /// with the expected arguments when optional arguments are given.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsWhenOptionalArgumentsAreGiven()
    {
        var delay = 42;
        var dependencies = new string[] { ExampleJidOther };
        var priority = 23;
        var retries = 7;
        var tags = new string[] { ExampleTag };
        var throttles = new string[] { ExampleThrottle };

        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RequeueJobAsync(
                className: ExampleClassName,
                data: ExampleData,
                delay: delay,
                dependencies: dependencies,
                jid: ExampleJid,
                priority: priority,
                retries: retries,
                queueName: ExampleQueueName,
                tags: tags,
                throttles: throttles,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.requeue",
                0,
                ExampleWorkerName,
                ExampleQueueName,
                ExampleJid,
                ExampleClassName,
                ExampleData,
                delay,
                "priority",
                priority,
                "retries",
                retries,
                "depends",
                JsonSerializer.Serialize(dependencies),
                "tags",
                JsonSerializer.Serialize(tags),
                "throttles",
                JsonSerializer.Serialize(throttles),
            ],
            returnValue: ExampleJid
        );
    }
}