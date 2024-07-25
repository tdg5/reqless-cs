using Reqless.Client;
using StackExchange.Redis;

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
    public async void ThrowsIfResultIsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetCompletedJobsAsync(),
                expectedArguments: ["jobs.completed", 0, 0, 25],
                returnValue: null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should throw if any
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetCompletedJobsAsync(),
                expectedArguments: ["jobs.completed", 0, 0, 25],
                returnValues: [RedisValue.Null]
            )
        );
        Assert.Equal(
            "Server returned unexpected null jid.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return an empty
    /// list when there are no completed jobs.
    /// </summary>
    [Fact]
    public async void ReturnsEmptyListWhenNoSuchJobs()
    {
        List<string> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetCompletedJobsAsync(limit: 25, offset: 0),
            expectedArguments: ["jobs.completed", 0, 0, 25],
            returnValues: []
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetCompletedJobsAsync"/> should return jids
    /// when there are completed jobs.
    /// </summary>
    [Fact]
    public async void ReturnsJidsWhenThereAreCompletedJobs()
    {
        List<string> jids = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetCompletedJobsAsync(
                limit: 25,
                offset: 0
            ),
            expectedArguments: ["jobs.completed", 0, 0, 25],
            returnValues: [ExampleJid, ExampleJidOther]
        );
        var expectedJids = new string[] { ExampleJid, ExampleJidOther };
        Assert.Equal(expectedJids.Length, jids.Count);
        Assert.Contains(jids[0], expectedJids);
        Assert.Contains(jids[1], expectedJids);
    }
}
