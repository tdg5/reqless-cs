using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

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
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueStatsAsync(queueName: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetQueueStatsAsync(emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueStatsAsync(ExampleQueueName, ExampleDate),
                expectedArguments: [
                    "queue.stats",
                    0,
                    ExampleQueueName,
                    ExampleDate.ToUnixTimeMilliseconds(),
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
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
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize queue stats JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueStatsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async void ReturnsValidResultFromTheServer()
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
            returnValue: queueStatsJson
        );
        Assert.Equivalent(expectedQueueStats, queueStats);
    }
}