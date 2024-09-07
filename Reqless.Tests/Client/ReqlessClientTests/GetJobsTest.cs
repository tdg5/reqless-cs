using System.Text.Json;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Client.Models;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobsAsync"/>.
/// </summary>
public class GetJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return the job returned
    /// by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsTheJobReturnedByTheServer()
    {
        var jobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var otherJobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJidOther));
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobsAsync(ExampleJid),
            expectedArguments: ["job.getMulti", 0, ExampleJid],
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
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty list if
    /// the server responds with an empty list.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyArrayIfServerRespondsWithEmptyArray()
    {
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobsAsync(ExampleJid),
            expectedArguments: ["job.getMulti", 0, ExampleJid],
            returnValue: "[]"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty list if
    /// the server responds with an empty object.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyArrayIfServerRespondsWithEmptyObject()
    {
        List<Job> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobsAsync(ExampleJid),
            expectedArguments: ["job.getMulti", 0, ExampleJid],
            returnValue: "{}"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the job JSON can't be
    /// deserialized into a job.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJobJsonIsInvalid()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid),
                expectedArguments: ["job.getMulti", 0, ExampleJid],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize jobs JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the given jids
    /// argument is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(null!)
            ),
            "jids"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if any of the given JIDs
    /// are null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyOfTheJidsAreNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid, null!)
            ),
            "jids"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the server returns
    /// null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTheServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid),
                expectedArguments: ["job.getMulti", 0, ExampleJid],
                returnValue: null
            )
        );
    }
}
