using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddTagToRecurringJobAsync"/>.
/// </summary>
public class AddTagToRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> throws if the
    /// given jid is null, or empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToRecurringJobAsync(invalidJid!, ExampleTag)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> throws if the
    /// given tag argument is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfTagIsNull()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToRecurringJobAsync(ExampleJid, invalidTag!)
            ),
            "tag"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagToRecurringJobAsync(
                ExampleJid,
                ExampleTag
            ),
            expectedArguments: [
                "recurringJob.addTag",
                0,
                ExampleJid,
                ExampleTag,
            ],
            returnValue: $"""["{ExampleTag}"]"""
        );
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }
}
