using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToRecurringJobAsync(invalidJid!, ExampleTag)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> throws if the
    /// given tag argument is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTagIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToRecurringJobAsync(ExampleJid, invalidTag!)),
            "tag");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToRecurringJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagToRecurringJobAsync(ExampleJid, ExampleTag),
            expectedArguments: ["recurringJob.addTag", 0, ExampleJid, ExampleTag],
            returnValue: $"""["{ExampleTag}"]""");
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }
}
