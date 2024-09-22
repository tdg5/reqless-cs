using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/>.
/// </summary>
public class RemoveTagFromRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if the
    /// given jid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(
                    jid: invalidJid!,
                    tag: ExampleTag
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> throws if the
    /// given tag argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTagIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveTagFromRecurringJobAsync(
                    jid: ExampleJid,
                    tag: invalidTag!
                )
            ),
            "tag"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
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
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> should throw
    /// if the server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
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
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveTagFromRecurringJobAsync"/> should throw
    /// if the JSON can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
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
