using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/>.
/// </summary>
public class GetThrottleLockOwnersTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/> should throw if
    /// throttle name is null, or empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottleName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockOwnersAsync(
                    throttleName: invalidThrottleName!
                )
            ),
            "throttleName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/> should throw if
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockOwnersAsync(ExampleThrottleName),
                expectedArguments: ["throttle.locks", 0, ExampleThrottleName],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/> should throw if
    /// server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetThrottleLockOwnersAsync(ExampleThrottleName),
                expectedArguments: ["throttle.locks", 0, ExampleThrottleName],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize throttle members JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetThrottleLockOwnersAsync"/> should return a
    /// valid result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        string[] expectedLockOwners = [ExampleJid, ExampleJidOther];
        var lockOwners = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetThrottleLockOwnersAsync(ExampleThrottleName),
            expectedArguments: ["throttle.locks", 0, ExampleThrottleName],
            returnValue: JsonSerializer.Serialize(expectedLockOwners)
        );
        Assert.Equivalent(expectedLockOwners, lockOwners);
    }
}
