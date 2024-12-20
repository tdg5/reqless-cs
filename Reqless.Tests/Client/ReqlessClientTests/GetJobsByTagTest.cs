using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobsByTagAsync"/>.
/// </summary>
public class GetJobsByTagTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should throw if tag is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ShouldThrowIfTagIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByTagAsync(tag: invalidTag!)),
            "tag");
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should throw if server
    /// returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ShouldThrowIfServerReturnsNull()
    {
        var limit = 25;
        var offset = 0;
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByTagAsync(
                    limit: limit, offset: offset, tag: ExampleTag),
                expectedArguments: [
                    "jobs.tagged",
                    0,
                    ExampleTag,
                    offset,
                    limit
                ],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should throw if JSON can't
    /// be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ShouldThrowIfJsonCannotBeDeserialized()
    {
        var limit = 25;
        var offset = 0;
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByTagAsync(
                    limit: limit, offset: offset, tag: ExampleTag),
                expectedArguments: [
                    "jobs.tagged",
                    0,
                    ExampleTag,
                    offset,
                    limit
                ],
                returnValue: "null"));
        Assert.Equal(
            "Failed to deserialize failed jobs query result JSON: null",
            exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByTagAsync"/> should return JidsResult.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ShouldReturnJidsResult()
    {
        var limit = 25;
        var offset = 0;
        var total = 21;
        var jids = new List<string> { "jid1", "jid2" };
        var jobsJson = JsonSerializer.Serialize(jids);
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobsByTagAsync(
                limit: limit, offset: offset, tag: ExampleTag),
            expectedArguments: [
                "jobs.tagged",
                0,
                ExampleTag,
                offset,
                limit
            ],
            returnValue: $$"""{"total":{{total}},"jobs":{{jobsJson}}}""");
        Assert.IsType<JidsResult>(result);
        Assert.Equal(total, result.Total);
        Assert.Equal(jids, result.Jids);
    }
}
