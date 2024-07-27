using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/>.
/// </summary>
public class AddTagsToRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if the
    /// given jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if the
    /// given tags argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagsIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if any of
    /// the tags are null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagsAreNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, [null!])
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if any of
    /// the tags are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagsAreEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddTagsToRecurringJobAsync(ExampleJid, [emptyString])
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'tags')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagsToRecurringJobAsync(
                ExampleJid,
                ExampleTag,
                ExampleTagOther
            ),
            expectedArguments: [
                "recurringJob.addTag",
                0,
                ExampleJid,
                ExampleTag,
                ExampleTagOther,
            ],
            returnValue: $"""["{ExampleTag}", "{ExampleTagOther}"]"""
        );
        Assert.Equivalent(
            new string[] { ExampleTag, ExampleTagOther },
            tags
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should throw if
    /// the server returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "recurringJob.addTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should throw if
    /// the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(
                    ExampleJid,
                    ExampleTag
                ),
                expectedArguments: [
                    "recurringJob.addTag",
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