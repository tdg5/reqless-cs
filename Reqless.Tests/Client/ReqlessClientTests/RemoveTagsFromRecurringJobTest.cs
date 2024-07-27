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
    /// the given jid is null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// the given tags argument is null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfTagsIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// any of the tags are null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfAnyTagsAreNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(ExampleJid, [null!])
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// any of the tags are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfAnyTagsAreEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagsFromRecurringJobAsync(ExampleJid, [emptyString])
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromRecurringJobAsync_CallsExecutorWithTheExpectedArguments()
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
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfServerReturnsNull()
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
    public async void RemoveTagsFromRecurringJobAsync_ThrowsIfJsonCannotBeDeserialized()
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