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
    /// is null, or empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if jid is null,
    /// empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "className"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if data is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
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