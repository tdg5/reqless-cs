using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetFailureGroupsCountsAsync"/>.
/// </summary>
public class GetFailureGroupsCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetFailureGroupsCountsAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var expectedResult = new Dictionary<string, int>() { { ExampleGroupName, 1 } };
        string failedCountsJson = JsonSerializer.Serialize(expectedResult);
        Dictionary<string, int> failedCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetFailureGroupsCountsAsync(),
            expectedArguments: ["failureGroups.counts", 0],
            returnValue: failedCountsJson);
        Assert.Equivalent(expectedResult, failedCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailureGroupsCountsAsync"/> throws if the
    /// server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetFailureGroupsCountsAsync(),
                expectedArguments: ["failureGroups.counts", 0],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetFailureGroupsCountsAsync"/> throws if the JSON
    /// can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetFailureGroupsCountsAsync(),
                expectedArguments: ["failureGroups.counts", 0],
                returnValue: "null"));
        Assert.Equal(
            "Failed to deserialize failed counts JSON: null",
            exception.Message);
    }
}
