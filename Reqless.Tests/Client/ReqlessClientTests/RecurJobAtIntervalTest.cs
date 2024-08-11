using System.Text.Json;
using Moq;
using Reqless.Client;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RecurJobAtIntervalAsync"/>.
/// </summary>
public class RecurJobAtIntervalTest : BaseReqlessClientTest
{
    private static readonly int ExampleIntervalSeconds = 60;

    private static readonly int ExampleInitialDelaySeconds = 30;

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should return the new
    /// expiration time.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedDefaultValuesForOptionalArguments()
    {
        // All this ceremony stems from the default jid being unpredictable.
        var executorMock = new Mock<RedisExecutor>();
        executorMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => "queue.recurAtInterval" == args[0] &&
                    0 == args[1] &&
                    ExampleQueueName == args[2] &&
                    JidRegex().IsMatch(((string?)args[3])!) &&
                    ExampleClassName == args[4] &&
                    ExampleData == args[5] &&
                    ExampleIntervalSeconds == args[6] &&
                    0 == args[7] &&
                    "backlog" == args[8] &&
                    0 == args[9] &&
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
        using (var subject = new PredictableNowReqlessClient(executorMock.Object))
        {
            string resultJid = await subject.RecurJobAtIntervalAsync(
                className: ExampleClassName,
                data: ExampleData,
                intervalSeconds: ExampleIntervalSeconds,
                queueName: ExampleQueueName
            );
            Assert.Equal(ExampleJid, resultJid);
        }
        executorMock.VerifyAll();
        executorMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    jid: ExampleJid,
                    queueName: ExampleQueueName
                ),
                expectedArguments: [
                    "queue.recurAtInterval",
                    0,
                    ExampleQueueName,
                    ExampleJid,
                    ExampleClassName,
                    ExampleData,
                    ExampleIntervalSeconds,
                    0,
                    "backlog",
                    0,
                    "priority",
                    0,
                    "retries",
                    5,
                    "tags",
                    "[]",
                    "throttles",
                    "[]"
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> returns a jid when
    /// successful.
    /// </summary>
    [Fact]
    public async Task ReturnsJidWhenSuccessful()
    {
        var maximumBacklog = 10;
        var priority = 20;
        var retries = 30;
        var tags = new string[] { "tag1", "tag2" };
        var throttles = new string[] { "throttle1", "throttle2" };

        var resultJid = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RecurJobAtIntervalAsync(
                className: ExampleClassName,
                data: ExampleData,
                initialDelaySeconds: ExampleInitialDelaySeconds,
                intervalSeconds: ExampleIntervalSeconds,
                jid: ExampleJid,
                maximumBacklog: maximumBacklog,
                priority: priority,
                queueName: ExampleQueueName,
                retries: retries,
                tags: tags,
                throttles: throttles
            ),
            expectedArguments: [
                "queue.recurAtInterval",
                0,
                ExampleQueueName,
                ExampleJid,
                ExampleClassName,
                ExampleData,
                ExampleIntervalSeconds,
                ExampleInitialDelaySeconds,
                "backlog",
                maximumBacklog,
                "priority",
                priority,
                "retries",
                retries,
                "tags",
                JsonSerializer.Serialize(tags),
                "throttles",
                JsonSerializer.Serialize(throttles)
            ],
            returnValue: ExampleJid
        );
        Assert.Equal(ExampleJid, resultJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: invalidQueueName!
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: ExampleQueueName
                )
            ),
            "className"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if data is null,
    /// empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: invalidData!,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: ExampleQueueName
                )
            ),
            "data"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if jid is empty or
    /// only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    jid: invalidJid,
                    queueName: ExampleQueueName
                )
            ),
            "jid"
        );
    }
}
