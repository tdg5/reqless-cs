using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Tests for the <see cref="ReqlessClient.GetTrackedJobsAsync"/>.
/// </summary>
public class GetTrackedJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetTrackedJobsAsync(),
                expectedArguments: ["jobs.tracked", 0],
                returnValue: null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should throw if the
    /// server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await WithClientWithExecutorMockForExpectedArguments(
            subject => Assert.ThrowsAsync<JsonException>(
                () => subject.GetTrackedJobsAsync()
            ),
            expectedArguments: [
                "jobs.tracked",
                0
            ],
            returnValue: "null"
        );
        Assert.Equal(
            "Failed to deserialize tracked jobs JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return the
    /// expected tracked jobs data.
    /// </summary>
    [Fact]
    public async Task ReturnsExpectedTrackedJobs()
    {
        var expiredJid = "expired-jid";
        var expiredJob = JobFactory.NewJob();
        var expiredJobJson = JsonSerializer.Serialize(expiredJob);
        var trackedJobsResult = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetTrackedJobsAsync(),
            expectedArguments: [
                "jobs.tracked",
                0
            ],
            returnValue: $$"""{"expired":["{{expiredJid}}"],"jobs":[{{expiredJobJson}}]}"""
        );
        Assert.Single(trackedJobsResult.ExpiredJids);
        Assert.Equal(expiredJid, trackedJobsResult.ExpiredJids[0]);
        Assert.Equivalent(expiredJob, trackedJobsResult.Jobs[0]);
    }
}
