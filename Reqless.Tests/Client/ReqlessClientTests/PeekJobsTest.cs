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
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PeekJobsAsync(null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should throw if the given
    /// queue name is empty or only whitespace.
    /// by the server.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.PeekJobsAsync(emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
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
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.PeekJobsAsync(ExampleQueueName);
                Assert.Equal(2, jobs.Length);
                var expectedJids = new string[] { ExampleJid, ExampleJidOther };
                foreach (var job in jobs)
                {
                    Assert.Contains(job.Jid, expectedJids);
                    Assert.IsType<Job>(job);
                }
            },
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: $"[{jobJson}, {otherJobJson}]"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return an empty array
    /// if the server responds with an empty array.
    /// /// </summary>
    [Fact]
    public async void ReturnsEmptyArrayIfServerRespondsWithEmptyArray()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.PeekJobsAsync(ExampleQueueName);
                Assert.Empty(jobs);
            },
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: "[]"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PeekJobsAsync"/> should return an empty array
    /// if the server responds with an empty object.
    /// </summary>
    [Fact]
    public async void ReturnsEmptyArrayIfServerRespondsWithEmptyObject()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job[] jobs = await subject.PeekJobsAsync(ExampleQueueName);
                Assert.Empty(jobs);
            },
            expectedArguments: ["queue.peek", 0, ExampleQueueName, 0, 25],
            returnValue: "{}"
        );
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