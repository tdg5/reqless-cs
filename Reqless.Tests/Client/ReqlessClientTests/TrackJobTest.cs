using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.TrackJobAsync"/>.
/// </summary>
public class TrackJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should throw if jid is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TrackJobAsync(jid: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should throw if jid is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.TrackJobAsync(jid: emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should throw if the server
    /// returns a null result.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TrackJobAsync(ExampleJid),
                expectedArguments: ["job.track", 0, ExampleJid],
                returnValue: null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> calls the executor with
    /// the expected arguments, returning true when the server returns 1.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsReturningTrue()
    {
        var trackedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TrackJobAsync(ExampleJid),
            expectedArguments: ["job.track", 0, ExampleJid],
            returnValue: 1
        );
        Assert.True(trackedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> calls the executor with
    /// the expected arguments, returning false when the server returns 0.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsReturningFalse()
    {
        var trackedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TrackJobAsync(ExampleJid),
            expectedArguments: ["job.track", 0, ExampleJid],
            returnValue: 0
        );
        Assert.False(trackedSuccessfully);
    }
}