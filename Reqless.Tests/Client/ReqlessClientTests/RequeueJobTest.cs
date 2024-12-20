using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using StackExchange.Redis;
using System.Text.Json;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if jid is null,
    /// empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    jid: invalidJid!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "className");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should throw if data is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RequeueJobAsync(
                    className: ExampleClassName,
                    data: invalidData!,
                    jid: ExampleJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should call the executor
    /// with the expected arguments when optional arguments are omitted.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArgumentsWhenOptionalArgumentsAreOmitted()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RequeueJobAsync(
                className: ExampleClassName,
                data: ExampleData,
                jid: ExampleJid,
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName),
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
            returnValue: ExampleJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RequeueJobAsync"/> should call the executor
    /// with the expected arguments when optional arguments are given.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArgumentsWhenOptionalArgumentsAreGiven()
    {
        var delay = 42;
        var dependencies = new string[] { ExampleJidOther };
        var priority = 23;
        var retries = 7;
        var tags = new string[] { ExampleTag };
        var throttles = new string[] { ExampleThrottleName };

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
                workerName: ExampleWorkerName),
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
            returnValue: ExampleJid);
    }
}
