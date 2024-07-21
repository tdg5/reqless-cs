using System.Text.Json;
using Reqless.Client;
using Reqless.Models;

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
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueCountsAsync(),
                expectedArguments: ["queue.counts", 0],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
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
    public async void ReturnsEmptyArrayWhenServerReturnsJsonObject()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var allQueueCounts = await subject.GetAllQueueCountsAsync();
                Assert.Empty(allQueueCounts);
            },
            expectedArguments: ["queue.counts", 0],
            returnValue: "{}"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueCountsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async void ReturnsValidResultFromTheServer()
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
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var allQueueCounts = await subject.GetAllQueueCountsAsync();
                Assert.Single(allQueueCounts);
                Assert.Equivalent(expectedCounts, allQueueCounts[0]);
            },
            expectedArguments: ["queue.counts", 0],
            returnValue: countsJson
        );
    }
}