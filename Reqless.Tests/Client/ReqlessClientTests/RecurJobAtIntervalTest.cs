using Moq;
using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using StackExchange.Redis;
using System.Text.Json;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedDefaultValuesForOptionalArguments()
    {
        // All this ceremony stems from the default jid being unpredictable.
        var executorMock = new Mock<RedisExecutor>();
        executorMock.Setup(
            mock => mock.ExecuteAsync(It.Is<RedisValue[]>(
                args => args[0] == "queue.recurAtInterval" &&
                    args[1] == 0 &&
                    args[2] == ExampleQueueName &&
                    JidRegex().IsMatch(((string?)args[3])!) &&
                    args[4] == ExampleClassName &&
                    args[5] == ExampleData &&
                    args[6] == ExampleIntervalSeconds &&
                    args[7] == 0 &&
                    args[8] == "backlog" &&
                    args[9] == 0 &&
                    args[10] == "priority" &&
                    args[11] == 0 &&
                    args[12] == "retries" &&
                    args[13] == 5 &&
                    args[14] == "tags" &&
                    args[15] == "[]" &&
                    args[16] == "throttles" &&
                    args[17] == "[]")))
            .Returns(Task.FromResult(RedisResult.Create((RedisValue)ExampleJid)));
        using (var subject = new PredictableNowReqlessClient(executorMock.Object))
        {
            string resultJid = await subject.RecurJobAtIntervalAsync(
                className: ExampleClassName,
                data: ExampleData,
                intervalSeconds: ExampleIntervalSeconds,
                queueName: ExampleQueueName);
            Assert.Equal(ExampleJid, resultJid);
        }
        executorMock.VerifyAll();
        executorMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    jid: ExampleJid,
                    queueName: ExampleQueueName),
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
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> returns a jid when
    /// successful.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
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
                throttles: throttles),
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
            returnValue: ExampleJid);
        Assert.Equal(ExampleJid, resultJid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if queueName is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if className is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfClassNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidClassName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: invalidClassName!,
                    data: ExampleData,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: ExampleQueueName)),
            "className");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if data is null,
    /// empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RecurJobAtIntervalAsync(
                    className: ExampleClassName,
                    data: invalidData!,
                    intervalSeconds: ExampleIntervalSeconds,
                    queueName: ExampleQueueName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RecurJobAtIntervalAsync"/> should throw if jid is empty or
    /// only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
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
                    queueName: ExampleQueueName)),
            "jid");
    }
}
