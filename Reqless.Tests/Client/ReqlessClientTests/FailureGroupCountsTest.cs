using System.Text.Json;
using Reqless.Client;

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
    public async void CallsExecutorWithTheExpectedArguments()
    {
        var expectedResult = new Dictionary<string, int>() {
            { ExampleGroup, 1 },
        };
        string failedCountsJson = JsonSerializer.Serialize(expectedResult);
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                Dictionary<string, int> failedCounts = await subject.FailureGroupsCountsAsync();
                Assert.Equivalent(expectedResult, failedCounts);
            },
            expectedArguments: ["failureGroups.counts", 0],
            returnValue: failedCountsJson
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> throws if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.FailureGroupsCountsAsync(),
                expectedArguments: ["failureGroups.counts", 0],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailureGroupsCountsAsync"/> throws if the JSON
    /// can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfJsonCannotBeDeserialized()
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
