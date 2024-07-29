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
    /// null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TrackJobAsync(jid: invalidJid!)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should throw if the server
    /// returns a null result.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
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
    public async Task CallsExecutorWithExpectedArgumentsReturningTrue()
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
    public async Task CallsExecutorWithExpectedArgumentsReturningFalse()
    {
        var trackedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TrackJobAsync(ExampleJid),
            expectedArguments: ["job.track", 0, ExampleJid],
            returnValue: 0
        );
        Assert.False(trackedSuccessfully);
    }
}
