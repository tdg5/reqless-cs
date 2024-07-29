using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PopJobAsync"/>.
/// </summary>
public class PopJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should throw if the
    /// given queue name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobAsync(
                    queueName: invalidQueueName!,
                    workerName: ExampleWorkerName
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobAsync(
                    queueName: ExampleQueueName,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var jobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var jobsJson = $"[{jobJson}]";
        Job? job = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PopJobAsync(
                queueName: ExampleQueueName,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "queue.pop",
                0,
                ExampleQueueName,
                ExampleWorkerName,
                1,

            ],
            returnValue: jobsJson
        );
        Assert.NotNull(job);
        Assert.IsType<Job>(job);
        Assert.Equal(ExampleJid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> should returns null if the
    /// server returns no jobs.
    /// </summary>
    [Fact]
    public async Task ReturnsNullIfTheServerReturnsNoJobs()
    {
        foreach (var emptyResult in new string[] { "[]", "{}" })
        {
            Job? job = await WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobAsync(
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    1,
                ],
                returnValue: emptyResult
            );
            Assert.Null(job);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> throws if the server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobAsync(
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    1,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PopJobAsync"/> throws if the job JSON can't be
    /// deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJobJSONCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PopJobAsync(
                    queueName: ExampleQueueName,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "queue.pop",
                    0,
                    ExampleQueueName,
                    ExampleWorkerName,
                    1,
                ],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize jobs JSON: null", exception.Message);
    }
}
