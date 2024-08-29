using System.Text.Json;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PopJobsAsync"/>.
/// </summary>
public class PopJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should throw if the
    /// given queue name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobsAsync(
                    limit: 10,
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobsAsync(
                    limit: 10,
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
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
    /// <see cref="ReqlessClient.PopJobsAsync"/> should return empty list if the
    /// server returns no jobs.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyListIfTheServerReturnsNoJobs()
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
    public async Task ThrowsIfTheServerReturnsNull()
    {
        var count = 2;
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
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
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobsAsync"/> throws if the job JSON can't be
    /// deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJobJSONCannotBeDeserialized()
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
