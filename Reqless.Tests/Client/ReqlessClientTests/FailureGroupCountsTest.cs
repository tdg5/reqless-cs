using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.FailureGroupsCountsAsync"/>.
/// </summary>
public class FailureGroupsCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var expectedResult = new Dictionary<string, int>() {
            { ExampleGroupName, 1 },
        };
        string failedCountsJson = JsonSerializer.Serialize(expectedResult);
        Dictionary<string, int> failedCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.FailureGroupsCountsAsync(),
            expectedArguments: ["failureGroups.counts", 0],
            returnValue: failedCountsJson
        );
        Assert.Equivalent(expectedResult, failedCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> throws if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailureGroupsCountsAsync(),
                expectedArguments: ["failureGroups.counts", 0],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> throws if the JSON
    /// can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailureGroupsCountsAsync(),
                expectedArguments: ["failureGroups.counts", 0],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize failed counts JSON: null",
            exception.Message
        );
    }
}
