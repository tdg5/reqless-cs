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
    /// given jid is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(invalidJid!, ExampleTag)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if the
    /// given tags argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagsIsNull()
    {
        await Scenario.ThrowsArgumentNullExceptionAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, null!)
            ),
            "tags"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if any of
    /// the tags are null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagsAreNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, [invalidTag!])
            ),
            "tags"
        );
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