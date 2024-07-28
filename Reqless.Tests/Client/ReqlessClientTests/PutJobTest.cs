using System.Text.Json;
using Moq;
using Reqless.Client;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

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
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
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
                throttles
            ),
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
            returnValue: ExampleJid
        );
        Assert.Equal(ExampleJid, jidFromPut);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should use expected defaults for
    /// optional arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedDefaults()
    {
        var executorMock = new Mock<RedisExecutor>();
        executorMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => "queue.put" == args[0] &&
                    0 == args[1] &&
                    ExampleWorkerName == args[2] &&
                    ExampleQueueName == args[3] &&
                    JidRegex().IsMatch(((string?)args[4])!) &&
                    ExampleClassName == args[5] &&
                    ExampleData == args[6] &&
                    0 == args[7] &&
                    "depends" == args[8] &&
                    "[]" == args[9] &&
                    "priority" == args[10] &&
                    0 == args[11] &&
                    "retries" == args[12] &&
                    5 == args[13] &&
                    "tags" == args[14] &&
                    "[]" == args[15] &&
                    "throttles" == args[16] &&
                    "[]" == args[17]
            ))
        ).Returns(
            Task.FromResult(RedisResult.Create((RedisValue)ExampleJid))
        );
        using var subject = new PredictableNowReqlessClient(executorMock.Object);
        string jidFromPut = await subject.PutJobAsync(
            ExampleWorkerName,
            ExampleQueueName,
            ExampleClassName,
            ExampleData
        );
        executorMock.VerifyAll();
        executorMock.VerifyNoOtherCalls();
        Assert.Equal(ExampleJid, jidFromPut);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> throws if the server returns
    /// null instead of a jid.
    /// </summary>
    [Fact]
    public async void ThrowsIfNullIsReturnedInsteadOfAJid()
    {
        var delay = 0;
        var priority = 0;
        var retries = 5;
        var dependencies = new string[] { "dependencies" };
        var tags = new string[] { "tags" };
        var throttles = new string[] { "throttles" };
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
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
                    throttles
                ),
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
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if workerName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "className"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if data is null,
    /// empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: invalidData!,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PutJobAsync"/> should throw if jid is empty or
    /// only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PutJobAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    jid: invalidJid,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                )
            ),
            "jid"
        );
    }
}