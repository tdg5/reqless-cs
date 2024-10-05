using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagsFromJobAsync"/>.
/// </summary>
public class RemoveTagsFromJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if the given
    /// jid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(
                    jid: invalidJid!,
                    tags: ExampleTag
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if the given
    /// tags argument is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTagsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(ExampleJid, null!)
            ),
            "tags"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if any of the
    /// tags are null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyTagsAreNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(
                    jid: ExampleJid,
                    tags: [invalidTag!]
                )
            ),
            "tags"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var leftoverTag = "leftover-tag";
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveTagsFromJobAsync(
                ExampleJid,
                ExampleTag,
                ExampleTagOther
            ),
            expectedArguments: [
                "job.removeTag",
                0,
                ExampleJid,
                ExampleTag,
                ExampleTagOther,
            ],
            returnValue: $"""["{leftoverTag}"]"""
        );
        Assert.Equivalent(new string[] { leftoverTag }, tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync" /> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(
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
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should throw if the
    /// JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(
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
