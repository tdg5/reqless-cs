using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddTagToJobAsync"/>.
/// </summary>
public class AddTagToJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given jid is
    /// null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(invalidJid!, ExampleTag)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given tag
    /// argument is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfTagIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(ExampleJid, invalidTag!)),
            "tag");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagToJobAsync(ExampleJid, ExampleTag),
            expectedArguments: ["job.addTag", 0, ExampleJid, ExampleTag],
            returnValue: $"""["{ExampleTag}"]""");
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }
}
