using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CancelJobAsync"/>.
/// </summary>
public class CancelJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CancelJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                bool cancelledSuccessfully = await subject.CancelJobAsync(jid: ExampleJid);
                Assert.True(cancelledSuccessfully);
            },
            expectedArguments: ["job.cancel", 0, ExampleJid],
            returnValue: ExampleJid
        );
    }
}
