using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetQueueStatsAsync"/>.
/// </summary>
public class GetQueueStatsTest : BaseReqlessClientTest
{
    /// <summary>
    /// An example date value to use in tests.
    /// </summary>
    public static readonly DateTimeOffset ExampleDate = DateTimeOffset.UtcNow;

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if queue
    /// name is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueStatsAsync(queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueStatsAsync(ExampleQueueName, ExampleDate),
                expectedArguments: [
                    "queue.stats",
                    0,
                    ExampleQueueName,
                    ExampleDate.ToUnixTimeMilliseconds(),
                ],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueStatsAsync(ExampleQueueName, ExampleDate),
                expectedArguments: [
                    "queue.stats",
                    0,
                    ExampleQueueName,
                    ExampleDate.ToUnixTimeMilliseconds(),
                ],
                returnValue: "null"));
        Assert.Equal("Failed to deserialize queue stats JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var dummyHistogramData = new int[148];
        Array.Fill(dummyHistogramData, 0);
        var expectedQueueStats = new QueueStats
        {
            Failed = 0,
            Failures = 0,
            Retries = 0,
            Run = new()
            {
                Count = 0,
                Histogram = dummyHistogramData,
                Mean = 0,
                StandardDeviation = 0,
            },
            Wait = new()
            {
                Count = 0,
                Histogram = dummyHistogramData,
                Mean = 0,
                StandardDeviation = 0,
            },
        };
        var queueStatsJson = JsonSerializer.Serialize(expectedQueueStats);
        var queueStats = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetQueueStatsAsync(ExampleQueueName, ExampleDate),
            expectedArguments: [
                "queue.stats",
                0,
                ExampleQueueName,
                ExampleDate.ToUnixTimeMilliseconds(),
            ],
            returnValue: queueStatsJson);
        Assert.Equivalent(expectedQueueStats, queueStats);
    }
}
