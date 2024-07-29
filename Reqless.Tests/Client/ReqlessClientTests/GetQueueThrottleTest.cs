using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetQueueThrottleAsync"/>.
/// </summary>
public class GetQueueThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetQueueThrottleAsync"/> should throw if queue
    /// name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueThrottleAsync(queueName: invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueThrottleAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueThrottleAsync(ExampleQueueName),
                expectedArguments: ["queue.throttle.get", 0, ExampleQueueName],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueThrottleAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueThrottleAsync(ExampleQueueName),
                expectedArguments: ["queue.throttle.get", 0, ExampleQueueName],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize throttle JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueThrottleAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var expectedThrottle = new Throttle
        {
            Id = $"ql:q:{ExampleQueueName}",
            Maximum = 42,
            Ttl = 60,
        };
        var throttleJson = JsonSerializer.Serialize(expectedThrottle);
        var throttle = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetQueueThrottleAsync(ExampleQueueName),
            expectedArguments: [
                "queue.throttle.get",
                0,
                ExampleQueueName,
            ],
            returnValue: throttleJson
        );
        Assert.Equivalent(expectedThrottle, throttle);
    }
}
