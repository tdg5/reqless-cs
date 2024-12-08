using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TrackJobAsync(jid: invalidJid!)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> should throw if the server
    /// returns a null result.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TrackJobAsync(ExampleJid),
                expectedArguments: ["job.track", 0, ExampleJid],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> calls the executor with
    /// the expected arguments, returning true when the server returns 1.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArgumentsReturningTrue()
    {
        var trackedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TrackJobAsync(ExampleJid),
            expectedArguments: ["job.track", 0, ExampleJid],
            returnValue: 1);
        Assert.True(trackedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TrackJobAsync"/> calls the executor with
    /// the expected arguments, returning false when the server returns 0.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArgumentsReturningFalse()
    {
        var trackedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TrackJobAsync(ExampleJid),
            expectedArguments: ["job.track", 0, ExampleJid],
            returnValue: 0);
        Assert.False(trackedSuccessfully);
    }
}
