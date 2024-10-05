using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagFromJobAsync"/>.
/// </summary>
public class RemoveTagFromJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if the given
    /// jid is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(invalidJid!, ExampleTag)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if the given
    /// tag argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTagIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(ExampleJid, invalidTag!)
            ),
            "tag"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveTagFromJobAsync(
                ExampleJid,
                ExampleTagOther
            ),
            expectedArguments: [
                "job.removeTag",
                0,
                ExampleJid,
                ExampleTagOther,
            ],
            returnValue: $"""["{ExampleTag}"]"""
        );
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "job.removeTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                    ExampleTagOther,
                ],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should throw if the
    /// JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "job.removeTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                ],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize tags JSON: null", exception.Message);
    }
}
