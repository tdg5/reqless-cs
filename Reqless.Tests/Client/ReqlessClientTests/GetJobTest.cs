using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobAsync"/>.
/// </summary>
public class GetJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> should return the job returned
    /// by the server.
    /// </summary>
    [Fact]
    public async void ReturnsTheJobReturnedByTheServer()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job? job = await subject.GetJobAsync(jid: ExampleJid);
                Assert.NotNull(job);
                Assert.Equal(ExampleJid, job.Jid);
            },
            expectedArguments: ["job.get", 0, ExampleJid],
            returnValue: JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid))
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> should return null if the server
    /// responds with null.
    /// </summary>
    [Fact]
    public async void ReturnsNullIfServerRespondsWithNull()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                Job? job = await subject.GetJobAsync(jid: ExampleJid);
                Assert.Null(job);
            },
            expectedArguments: ["job.get", 0, ExampleJid],
            returnValue: RedisValue.Null
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> throws if the job JSON can't be
    /// deserialized into a job.
    /// </summary>
    [Fact]
    public async void ThrowsIfJobJsonIsInvalid()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobAsync(jid: ExampleJid),
                expectedArguments: ["job.get", 0, ExampleJid],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize job JSON: null", exception.Message);
    }
}