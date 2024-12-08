using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(invalidJid!, ExampleTag)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if the
    /// given tags argument is null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTagsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, null!)),
            "tags");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> throws if any of
    /// the tags are null, empty, or composed entirely of whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfAnyTagsAreNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, [invalidTag!])),
            "tags");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagsToRecurringJobAsync(
                ExampleJid, ExampleTag, ExampleTagOther),
            expectedArguments: [
                "recurringJob.addTag",
                0,
                ExampleJid,
                ExampleTag,
                ExampleTagOther,
            ],
            returnValue: $"""["{ExampleTag}", "{ExampleTagOther}"]""");
        Assert.Equivalent(new string[] { ExampleTag, ExampleTagOther }, tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should throw if
    /// the server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, ExampleTag),
                expectedArguments: ["recurringJob.addTag", 0, ExampleJid, ExampleTag],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagsToRecurringJobAsync"/> should throw if
    /// the JSON can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagsToRecurringJobAsync(ExampleJid, ExampleTag),
                expectedArguments: ["recurringJob.addTag", 0, ExampleJid, ExampleTag],
                returnValue: "null"));
        Assert.Equal("Failed to deserialize tags JSON: null", exception.Message);
    }
}
