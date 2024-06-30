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
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(null!, ExampleTag)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given jid is
    /// empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddTagToJobAsync(emptyString, ExampleTag)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if the given tags
    /// argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfTagIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddTagToJobAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'tag')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> throws if any of the tags
    /// are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyTagIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.AddTagToJobAsync(ExampleJid, emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'tag')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddTagToJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                List<string> tags = await subject.AddTagToJobAsync(
                    ExampleJid,
                    ExampleTag
                );
                Assert.Equivalent(new string[] { ExampleTag }, tags);
            },
            expectedArguments: [
                "job.addTag",
                0,
                ExampleJid,
                ExampleTag,
            ],
            returnValue: $"""["{ExampleTag}"]"""
        );
    }
}
