using Reqless.Client;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Tests for the <see cref="ReqlessClient.GetWorkerJobsAsync"/>.
/// </summary>
public class GetWorkerJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetWorkerJobsAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetWorkerJobsAsync(ExampleWorkerName),
                expectedArguments: ["worker.jobs", 0, ExampleWorkerName],
                returnValue: null));
        Assert.Equal(
            "Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetWorkerJobsAsync"/> should throw if the
    /// server returns JSON that can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTheServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await WithClientWithExecutorMockForExpectedArguments(
            subject => Assert.ThrowsAsync<JsonException>(
                () => subject.GetWorkerJobsAsync(ExampleWorkerName)),
            expectedArguments: [
                "worker.jobs",
                0,
                ExampleWorkerName,
            ],
            returnValue: "null");
        Assert.Equal(
            "Failed to deserialize worker jobs JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetWorkerJobsAsync"/> should return the
    /// expected worker jobs data.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ReturnsExpectedWorkerJobs()
    {
        var expiredJid = "expired-jid";
        var workerJobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetWorkerJobsAsync(ExampleWorkerName),
            expectedArguments: [
                "worker.jobs",
                0,
                ExampleWorkerName,
            ],
            returnValue: $$"""{"jobs":["{{ExampleJid}}"], "stalled":["{{expiredJid}}"]}""");
        Assert.Single(workerJobs.Stalled);
        Assert.Equal(expiredJid, workerJobs.Stalled[0]);
        Assert.Single(workerJobs.Jobs);
        Assert.Equal(ExampleJid, workerJobs.Jobs[0]);
    }
}
