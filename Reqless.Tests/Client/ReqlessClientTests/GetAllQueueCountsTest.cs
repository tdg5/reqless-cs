using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllQueueCountsAsync"/>.
/// </summary>
public class GetAllQueueCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueCountsAsync(),
                expectedArguments: ["queue.counts", 0],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueCountsAsync(),
                expectedArguments: ["queue.counts", 0],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize all queue counts JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should return an
    /// empty array when the server returns an empty JSON object.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyArrayWhenServerReturnsJsonObject()
    {
        var allQueueCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllQueueCountsAsync(),
            expectedArguments: ["queue.counts", 0],
            returnValue: "{}"
        );
        Assert.Empty(allQueueCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should return a valid
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
        var countsJson = JsonSerializer.Serialize<QueueCounts[]>([expectedCounts]);
        var allQueueCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllQueueCountsAsync(),
            expectedArguments: ["queue.counts", 0],
            returnValue: countsJson
        );
        Assert.Single(allQueueCounts);
        Assert.Equivalent(expectedCounts, allQueueCounts[0]);
    }
}
