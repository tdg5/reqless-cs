using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/>.
/// </summary>
public class RemoveTagFromRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if the
    /// given jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if the
    /// given jid is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagFromRecurringJobAsync(emptyString, ExampleTag)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if the
    /// given tags argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tag')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if any
    /// of the tags are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.RemoveTagFromRecurringJobAsync(ExampleJid, emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'tag')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveTagFromRecurringJobAsync(
                ExampleJid,
                ExampleTagOther
            ),
            expectedArguments: [
                "recurringJob.removeTag",
                0,
                ExampleJid,
                ExampleTagOther,
            ],
            returnValue: $"""["{ExampleTag}"]"""
        );
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync" /> should
    /// throw if the server returns null.
    /// </summary>
    [Fact]
    public async void RemoveTagFromRecurringJobAsync_ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(
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
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> should throw
    /// if the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async void RemoveTagFromRecurringJobAsync_ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(
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