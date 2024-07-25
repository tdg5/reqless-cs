using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PopJobsAsync"/>.
/// </summary>
public class PopJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        var count = 2;
        var jobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var otherJobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var jobsJson = $"[{jobJson},{otherJobJson}]";
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PopJobsAsync(
                limit: count,
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "queue.pop",
                0,
                ExampleQueueName,
                ExampleWorkerName,
                count,
            ],
            returnValue: jobsJson
        );
        Assert.Equal(count, jobs.Count);
        var job = jobs[0];
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(ExampleJid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should returns null if the
    /// server returns no jobs.
    /// </summary>
    [Fact]
    public async void ReturnsNullIfTheServerReturnsNoJobs()
    {
        var count = 2;
        foreach (var emptyResult in new string[] { "[]", "{}" })
        {
            List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobsAsync(
                    limit: count,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    count,
                ],
                returnValue: emptyResult
            );
            Assert.Empty(jobs);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> throws if the server returns
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTheServerReturnsNull()
    {
        var count = 2;
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobsAsync(
                    limit: count,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    count,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> throws if the job JSON can't be
    /// deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfJobJSONCannotBeDeserialized()
    {
        var count = 2;
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobsAsync(
                    limit: count,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    count,
                ],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize jobs JSON: null", exception.Message);
    }
}