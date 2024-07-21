using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;
using StackExchange.Redis;

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
    public async void ReturnsTheJobReturnedByTheServer()
    {
        var jobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid));
        var otherJobJson = JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJidOther));
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.GetJobsAsync(ExampleJid);
                Assert.Equal(2, jobs.Length);
                var expectedJids = new string[] { ExampleJid, ExampleJidOther };
                foreach (var job in jobs)
                {
                    Assert.Contains(job.Jid, expectedJids);
                    Assert.IsType<Job>(job);
                }
            },
            expectedArguments: ["job.getMulti", 0, ExampleJid],
            returnValue: $"[{jobJson}, {otherJobJson}]"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty list if
    /// the server responds with an empty list.
    /// </summary>
    [Fact]
    public async void ReturnsNullIfServerRespondsWithEmptyArray()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.GetJobsAsync(ExampleJid);
                Assert.Empty(jobs);
            },
            expectedArguments: ["job.getMulti", 0, ExampleJid],
            returnValue: "[]"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> should return an empty list if
    /// the server responds with an empty object.
    /// </summary>
    [Fact]
    public async void ReturnsNullIfServerRespondsWithEmptyObject()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.GetJobsAsync(ExampleJid);
                Assert.Empty(jobs);
            },
            expectedArguments: ["job.getMulti", 0, ExampleJid],
            returnValue: "{}"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the job JSON can't be
    /// deserialized into a job.
    /// </summary>
    [Fact]
    public async void ThrowsIfJobJsonIsInvalid()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid),
                expectedArguments: ["job.getMulti", 0, ExampleJid],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize job JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the given jids
    /// argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidsIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jids')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if any of the given JIDs
    /// are null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyOfTheJidsAreNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid, null!),
                expectedArguments: ["job.getMulti", 0, ExampleJid, RedisValue.Null],
                returnValue: null
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'jids')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsAsync"/> throws if the server returns
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTheServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsAsync(ExampleJid),
                expectedArguments: ["job.getMulti", 0, ExampleJid],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }
}
