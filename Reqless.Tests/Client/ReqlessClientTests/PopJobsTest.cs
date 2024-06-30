using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;
using StackExchange.Redis;

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
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                List<Job> jobs = await subject.PopJobsAsync(
                    limit: count,
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                );
                Assert.Equal(count, jobs.Count);
                var job = jobs[0];
                Assert.NotNull(job);
                Assert.IsType<Job>(job);
                Assert.Equal(ExampleJid, job.Jid);
            },
            expectedArguments: [
                "queue.pop",
                0,
                ExampleQueueName,
                ExampleWorkerName,
                count,
            ],
            returnValue: jobsJson
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should returns null if the
    /// server returns no jobs.
    /// </summary>
    [Fact]
    public async void ReturnsNullIfTheServerReturnsNoJobs()
    {
        var count = 2;
        foreach (var emptyResult in new RedisValue[] { "[]", "{}" })
        {
            await WithClientWithExecutorMockForExpectedArguments(
                async subject =>
                {
                    List<Job> jobs = await subject.PopJobsAsync(
                        limit: count,
                        queueName: ExampleQueueName,
                        workerName: ExampleWorkerName
                    );
                    Assert.Empty(jobs);
                },
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    count,
                ],
                returnValue: emptyResult
            );
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
                returnValue: RedisValue.Null
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
