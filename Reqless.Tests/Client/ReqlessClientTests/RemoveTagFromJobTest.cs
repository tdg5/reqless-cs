using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagFromJobAsync"/> and <see
/// cref="ReqlessClient.RemoveTagsFromJobAsync"/>.
/// </summary>
public class RemoveTagFromJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if the given
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if the given
    /// jid is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagFromJobAsync(emptyString, ExampleTag)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if the given
    /// tags argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tag')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> throws if any of the
    /// tags are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagFromJobAsync(ExampleJid, emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'tag')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
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
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if the given
    /// jid is null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if the given
    /// tags argument is null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ThrowsIfTagsIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if any of the
    /// tags are null.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ThrowsIfAnyTagsAreNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(ExampleJid, [null!])
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> throws if any of the
    /// tags are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ThrowsIfAnyTagsAreEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagsFromJobAsync(ExampleJid, [emptyString])
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_CallsExecutorWithTheExpectedArguments()
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
    public async void RemoveTagsFromJobAsync_ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromJobAsync(
                    ExampleJid,
                    ExampleTag
                )
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromJobAsync"/> should throw if the
    /// JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async void RemoveTagsFromJobAsync_ThrowsIfJsonCannotBeDeserialized()
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