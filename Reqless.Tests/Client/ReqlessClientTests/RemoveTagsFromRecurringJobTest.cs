using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/>.
/// </summary>
public class RemoveTagsFromRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// the given jid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: invalidJid!,
                    tags: ExampleTag
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// the given tags argument is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTagsIsNull()
    {
        await Scenario.ThrowsArgumentNullExceptionAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: ExampleJid,
                    tags: null!
                )
            ),
            "tags"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// any of the tags are null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyTagsAreNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: ExampleJid,
                    tags: [invalidTag!]
                )
            ),
            "tags"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var leftoverTag = "leftover-tag";
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveTagsFromRecurringJobAsync(
                ExampleJid,
                ExampleTag,
                ExampleTagOther
            ),
            expectedArguments: [
                "recurringJob.removeTag",
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
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync" /> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "recurringJob.removeTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                    ExampleTagOther,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> should throw
    /// if the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "recurringJob.removeTag",
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
