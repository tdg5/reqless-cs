using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/>.
/// </summary>
public class GetThrottleLockWaitersTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/> should throw if
    /// throttle name is null, or empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottleName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockWaitersAsync(
                    throttleName: invalidThrottleName!
                )
            ),
            "throttleName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/> should throw if
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockWaitersAsync(ExampleThrottleName),
                expectedArguments: ["throttle.pending", 0, ExampleThrottleName],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/> should throw if
    /// server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockWaitersAsync(ExampleThrottleName),
                expectedArguments: ["throttle.pending", 0, ExampleThrottleName],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize throttle members JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockWaitersAsync"/> should return a
    /// valid result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        string[] expectedLockWaiters = [ExampleJid, ExampleJidOther];
        var lockWaiters = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetThrottleLockWaitersAsync(ExampleThrottleName),
            expectedArguments: ["throttle.pending", 0, ExampleThrottleName],
            returnValue: JsonSerializer.Serialize(expectedLockWaiters)
        );
        Assert.Equivalent(expectedLockWaiters, lockWaiters);
    }
}