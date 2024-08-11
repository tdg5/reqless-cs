using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetCompletedJobsAsync"/>.
/// </summary>
public class GetCompletedJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should throw if result
    /// is null.
    /// </summary>
    [Fact]
    public async Task ThrowsWhenServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetCompletedJobsAsync(),
                expectedArguments: ["jobs.completed", 0, 0, 25],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should throw if result
    /// JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfResultJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetCompletedJobsAsync(),
                expectedArguments: ["jobs.completed", 0, 0, 25],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should throw if any
    /// jid is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyJidIsNull()
    {
        await Scenario.ThrowsWhenOperationEncountersElementThatIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetCompletedJobsAsync(),
                expectedArguments: ["jobs.completed", 0, 0, 25],
                returnValue: $"[{JsonSerializer.Serialize(invalidJid)}]"
            ),
            "jidsResult"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return an empty
    /// list when there are no completed jobs.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyListWhenNoSuchJobs()
    {
        List<string> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetCompletedJobsAsync(limit: 25, offset: 0),
            expectedArguments: ["jobs.completed", 0, 0, 25],
            returnValue: "[]"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return jids
    /// when there are completed jobs.
    /// </summary>
    [Fact]
    public async Task ReturnsJidsWhenThereAreCompletedJobs()
    {
        List<string> jids = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetCompletedJobsAsync(
                limit: 25,
                offset: 0
            ),
            expectedArguments: ["jobs.completed", 0, 0, 25],
            returnValue: $"""["{ExampleJid}","{ExampleJidOther}"]"""
        );
        var expectedJids = new string[] { ExampleJid, ExampleJidOther };
        Assert.Equal(expectedJids.Length, jids.Count);
        Assert.Contains(jids[0], expectedJids);
        Assert.Contains(jids[1], expectedJids);
    }
}
