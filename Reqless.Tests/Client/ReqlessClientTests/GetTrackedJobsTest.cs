using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers.Factories;
using StackExchange.Redis;

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
    public async void ThrowsIfTheServerReturnsNull()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await subject.GetTrackedJobsAsync()
                );
                Assert.Equal(
                    "Server returned unexpected null result.",
                    exception.Message
                );
            },
            expectedArguments: [
                "jobs.tracked",
                0
            ],
            returnValue: null
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should throw if the
    /// server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfTheServerReturnsJsonThatCannotBeDeserialized()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var exception = await Assert.ThrowsAsync<JsonException>(
                    async () => await subject.GetTrackedJobsAsync()
                );
                Assert.Equal(
                    "Failed to deserialize tracked jobs JSON: null",
                    exception.Message
                );
            },
            expectedArguments: [
                "jobs.tracked",
                0
            ],
            returnValue: "null"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTrackedJobsAsync"/> should return the
    /// expected tracked jobs data.
    /// </summary>
    [Fact]
    public async void ReturnsExpectedTrackedJobs()
    {
        var expiredJid = "expired-jid";
        var expiredJob = JobFactory.NewJob();
        var expiredJobJson = JsonSerializer.Serialize(expiredJob);
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var trackedJobsResult = await subject.GetTrackedJobsAsync();
                Assert.Single(trackedJobsResult.ExpiredJids);
                Assert.Equal(expiredJid, trackedJobsResult.ExpiredJids[0]);
                Assert.Equivalent(expiredJob, trackedJobsResult.Jobs[0]);
            },
            expectedArguments: [
                "jobs.tracked",
                0
            ],
            returnValue: $$"""{"expired":["{{expiredJid}}"],"jobs":[{{expiredJobJson}}]}"""
        );
    }
}