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
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfThrottleNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleAsync(throttleName: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'throttleName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should throw if throttle
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfThrottleNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetThrottleAsync(emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'throttleName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
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
    /// retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
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
    public async void ReturnsValidResultFromTheServer()
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