using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(ExampleQueueName),
                expectedArguments: ["queue.counts", 0, ExampleQueueName],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(ExampleQueueName),
                expectedArguments: ["queue.counts", 0, ExampleQueueName],
                returnValue: "null"));
        Assert.Equal("Failed to deserialize queue counts JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
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
            Waiting = 0,
        };
        var countsJson = JsonSerializer.Serialize(expectedCounts);
        var counts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetQueueCountsAsync(ExampleQueueName),
            expectedArguments: ["queue.counts", 0, ExampleQueueName],
            returnValue: countsJson);
        Assert.Equivalent(expectedCounts, counts);
    }
}
