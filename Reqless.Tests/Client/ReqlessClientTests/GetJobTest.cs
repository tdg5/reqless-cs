using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;
using Reqless.Tests.TestHelpers.Factories;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobAsync"/>.
/// </summary>
public class GetJobTest : BaseReqlessClientTest
{

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given jid is
    /// null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobAsync(invalidJid!)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> should return the job returned
    /// by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsTheJobReturnedByTheServer()
    {
        Job? job = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobAsync(jid: ExampleJid),
            expectedArguments: ["job.get", 0, ExampleJid],
            returnValue: JobFactory.JobJson(jid: Maybe<string?>.Some(ExampleJid))
        );
        Assert.NotNull(job);
        Assert.Equal(ExampleJid, job.Jid);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> should return null if the server
    /// responds with null.
    /// </summary>
    [Fact]
    public async Task ReturnsNullIfServerRespondsWithNull()
    {
        Job? job = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobAsync(jid: ExampleJid),
            expectedArguments: ["job.get", 0, ExampleJid],
            returnValue: null
        );
        Assert.Null(job);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobAsync"/> throws if the job JSON can't be
    /// deserialized into a job.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJobJsonIsInvalid()
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
