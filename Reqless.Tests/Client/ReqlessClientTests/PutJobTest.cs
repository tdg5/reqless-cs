using Moq;
using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using StackExchange.Redis;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PopJobAsync"/>.
/// </summary>
public class PutJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var delay = 0;
        var priority = 0;
        var retries = 5;
        var dependencies = new string[] { "dependencies" };
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };
        string jidFromPut = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PutJobAsync(
                ExampleWorkerName,
                ExampleQueueName,
                ExampleClassName,
                ExampleData,
                delay,
                ExampleJid,
                priority,
                retries,
                dependencies,
                tags,
                throttles),
            expectedArguments: [
                "queue.put",
                0,
                ExampleWorkerName,
                ExampleQueueName,
                ExampleJid,
                ExampleClassName,
                ExampleData,
                delay,
                "depends",
                JsonSerializer.Serialize(dependencies),
                "priority",
                priority,
                "retries",
                retries,
                "tags",
                JsonSerializer.Serialize(tags),
                "throttles",
                JsonSerializer.Serialize(throttles),
            ],
            returnValue: ExampleJid);
        Assert.Equal(ExampleJid, jidFromPut);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should use expected defaults for
    /// optional arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedDefaults()
    {
        var executorMock = new Mock<RedisExecutor>();
        executorMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args[0] == "queue.put" &&
                    args[1] == 0 &&
                    args[2] == ExampleWorkerName &&
                    args[3] == ExampleQueueName &&
                    JidRegex().IsMatch(((string?)args[4])!) &&
                    args[5] == ExampleClassName &&
                    args[6] == ExampleData &&
                    args[7] == 0 &&
                    args[8] == "depends" &&
                    args[9] == "[]" &&
                    args[10] == "priority" &&
                    args[11] == 0 &&
                    args[12] == "retries" &&
                    args[13] == 5 &&
                    args[14] == "tags" &&
                    args[15] == "[]" &&
                    args[16] == "throttles" &&
                    args[17] == "[]")))
            .Returns(Task.FromResult(RedisResult.Create((RedisValue)ExampleJid)));
        using var subject = new PredictableNowReqlessClient(executorMock.Object);
        string jidFromPut = await subject.PutJobAsync(
            ExampleWorkerName, ExampleQueueName, ExampleClassName, ExampleData);
        executorMock.VerifyAll();
        executorMock.VerifyNoOtherCalls();
        Assert.Equal(ExampleJid, jidFromPut);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> throws if the server returns
    /// null instead of a jid.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfNullIsReturnedInsteadOfAJid()
    {
        var delay = 0;
        var priority = 0;
        var retries = 5;
        var dependencies = new string[] { "dependencies" };
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    ExampleWorkerName,
                    ExampleQueueName,
                    ExampleClassName,
                    ExampleData,
                    delay,
                    ExampleJid,
                    priority,
                    retries,
                    dependencies,
                    tags,
                    throttles),
                expectedArguments: [
                    "queue.put",
                    0,
                    ExampleWorkerName,
                    ExampleQueueName,
                    ExampleJid,
                    ExampleClassName,
                    ExampleData,
                    delay,
                    "depends",
                    JsonSerializer.Serialize(dependencies),
                    "priority",
                    priority,
                    "retries",
                    retries,
                    "tags",
                    JsonSerializer.Serialize(tags),
                    "throttles",
                    JsonSerializer.Serialize(throttles),
                ],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if workerName is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "className");
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if data is null,
    /// empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: invalidData!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if jid is empty or
    /// only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    jid: invalidJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName)),
            "jid");
    }
}
