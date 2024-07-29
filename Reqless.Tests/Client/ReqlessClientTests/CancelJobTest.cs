using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelJobAsync(jid: invalidJid!)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        bool cancelledSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CancelJobAsync(jid: ExampleJid),
            expectedArguments: ["job.cancel", 0, ExampleJid],
            returnValue: ExampleJid
        );
        Assert.True(cancelledSuccessfully);
    }
}
