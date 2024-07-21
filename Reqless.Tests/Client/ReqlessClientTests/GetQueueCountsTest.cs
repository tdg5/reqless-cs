using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetQueueCountsAsync"/>.
/// </summary>
public class GetQueueCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if queue
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(queueName: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetQueueCountsAsync(emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueCountsAsync(ExampleQueueName),
                expectedArguments: [
                    "queue.counts",
                    0,
                    ExampleQueueName,
                ],
                returnValue: RedisValue.Null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueCountsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
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
        var countsJson = JsonSerializer.Serialize(expectedCounts);
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var counts = await subject.GetQueueCountsAsync(ExampleQueueName);
                Assert.Equivalent(expectedCounts, counts);
            },
            expectedArguments: [
                "queue.counts",
                0,
                ExampleQueueName,
            ],
            returnValue: countsJson
        );
    }
}