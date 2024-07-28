using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(invalidJid!, ExampleTag)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given tag
    /// argument is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidTag) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(ExampleJid, invalidTag!)
            ),
            "tag"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddTagToJobAsync(
                ExampleJid,
                ExampleTag
            ),
            expectedArguments: [
                "job.addTag",
                0,
                ExampleJid,
                ExampleTag,
            ],
            returnValue: $"""["{ExampleTag}"]"""
        );
        Assert.Equivalent(new string[] { ExampleTag }, tags);
    }
}