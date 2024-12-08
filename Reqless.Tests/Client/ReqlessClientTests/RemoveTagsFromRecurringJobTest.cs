using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: invalidJid!, tags: ExampleTag)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// the given tags argument is null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTagsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: ExampleJid, tags: null!)),
            "tags");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> throws if
    /// any of the tags are null, empty, or composed entirely of whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfAnyTagsAreNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    jid: ExampleJid, tags: [invalidTag!])),
            "tags");
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var leftoverTag = "leftover-tag";
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveTagsFromRecurringJobAsync(
                ExampleJid, ExampleTag, ExampleTagOther),
            expectedArguments: [
                "recurringJob.removeTag",
                0,
                ExampleJid,
                ExampleTag,
                ExampleTagOther,
            ],
            returnValue: $"""["{leftoverTag}"]""");
        Assert.Equivalent(new string[] { leftoverTag }, tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync" /> should throw if the
    /// server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    ExampleJid, ExampleTag),
                expectedArguments: [
                    "recurringJob.removeTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                    ExampleTagOther,
                ],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagsFromRecurringJobAsync"/> should throw
    /// if the JSON can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagsFromRecurringJobAsync(
                    ExampleJid, ExampleTag),
                expectedArguments: [
                    "recurringJob.removeTag",
                    0,
                    ExampleJid,
                    ExampleTag,
                ],
                returnValue: "null"));
        Assert.Equal("Failed to deserialize tags JSON: null", exception.Message);
    }
}
