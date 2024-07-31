using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetThrottleAsync"/>.
/// </summary>
public class GetThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should throw if throttle
    /// name is null, or empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottleName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleAsync(throttleName: invalidThrottleName!)
            ),
            "throttleName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleAsync(ExampleThrottleName),
                expectedArguments: ["throttle.get", 0, ExampleThrottleName],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should throw if server
    /// returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleAsync(ExampleThrottleName),
                expectedArguments: ["throttle.get", 0, ExampleThrottleName],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize throttle JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var expectedThrottle = new Throttle
        {
            Id = ExampleThrottleName,
            Maximum = 42,
            Ttl = 60,
        };
        var throttleJson = JsonSerializer.Serialize(expectedThrottle);
        var throttle = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetThrottleAsync(ExampleThrottleName),
            expectedArguments: ["throttle.get", 0, ExampleThrottleName],
            returnValue: throttleJson
        );
        Assert.Equivalent(expectedThrottle, throttle);
    }
}