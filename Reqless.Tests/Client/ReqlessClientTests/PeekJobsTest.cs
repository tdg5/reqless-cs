using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PeekJobsAsync"/>.
/// </summary>
public class PeekJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should throw if the queue name
    /// is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PeekJobsAsync(invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return the job returned
    /// by the server.
    /// </summary>
    [Fact]
    public async void ReturnsTheJobReturnedByTheServer()
    {
        var jobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var otherJobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJidOther));
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PeekJobsAsync(ExampleQueueName),
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: $"[{jobJson}, {otherJobJson}]"
        );
        Assert.Equal(2, jobs.Count);
        var expectedJids = new string[] { ExampleJid, ExampleJidOther };
        foreach (var job in jobs)
        {
            Assert.Contains(job.Jid, expectedJids);
            Assert.IsType<Job>(job);
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return an empty array
    /// if the server responds with an empty array.
    /// /// </summary>
    [Fact]
    public async void ReturnsEmptyArrayIfServerRespondsWithEmptyArray()
    {
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PeekJobsAsync(ExampleQueueName),
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: "[]"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return an empty array
    /// if the server responds with an empty object.
    /// </summary>
    [Fact]
    public async void ReturnsEmptyArrayIfServerRespondsWithEmptyObject()
    {
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PeekJobsAsync(ExampleQueueName),
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: "{}"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> throws if the server returns
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PeekJobsAsync(ExampleQueueName),
                expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> throws if the job JSON can't be
    /// deserialized into a job.
    /// </summary>
    [Fact]
    public async void ThrowsIfJobJsonIsInvalid()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PeekJobsAsync(ExampleQueueName),
                expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize jobs JSON: null", exception.Message);
    }
}