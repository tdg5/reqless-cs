using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetQueueCountsAsync"/>.
/// </summary>
public class GetQueueCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if queue
    /// name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(queueName: invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(ExampleQueueName),
                expectedArguments: [
                    "queue.counts",
                    0,
                    ExampleQueueName,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(ExampleQueueName),
                expectedArguments: [
                    "queue.counts",
                    0,
                    ExampleQueueName,
                ],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize queue counts JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var expectedCounts = new QueueCounts
        {
            Depends = 0,
            QueueName = ExampleQueueName,
            Paused = false,
            Recurring = 0,
            Running = 0,
            Scheduled = 0,
            Stalled = 0,
            Throttled = 0,
            Waiting = 0
        };
        var countsJson = JsonSerializer.Serialize(expectedCounts);
        var counts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetQueueCountsAsync(ExampleQueueName),
            expectedArguments: [
                "queue.counts",
                0,
                ExampleQueueName,
            ],
            returnValue: countsJson
        );
        Assert.Equivalent(expectedCounts, counts);
    }
}
