using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CancelJobAsync"/>.
/// </summary>
public class CancelJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should throw if the given jid
    /// is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelJobAsync(jid: invalidJid!)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        bool cancelledSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CancelJobAsync(jid: ExampleJid),
            expectedArguments: ["job.cancel", 0, ExampleJid],
            returnValue: ExampleJid);
        Assert.True(cancelledSuccessfully);
    }
}
