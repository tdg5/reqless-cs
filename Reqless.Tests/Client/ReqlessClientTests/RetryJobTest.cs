using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RetryJobAsync"/>.
/// </summary>
public class RetryJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should call Executor with
    /// the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        var delay = 0;

        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                bool retriedSuccessfully = await subject.RetryJobAsync(
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    ExampleGroup,
                    ExampleMessage,
                    delay
                );
                Assert.True(retriedSuccessfully);
            },
            expectedArguments: [
                "job.retry",
                0,
                ExampleJid,
                ExampleQueueName,
                ExampleWorkerName,
                delay,
                ExampleGroup,
                ExampleMessage,
            ],
            returnValue: 1
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RetryJobAsync"/> should throw if server returns
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var delay = 0;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RetryJobAsync(
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    ExampleGroup,
                    ExampleMessage,
                    delay
                ),
                expectedArguments: [
                    "job.retry",
                    0,
                    ExampleJid,
                    ExampleQueueName,
                    ExampleWorkerName,
                    delay,
                    ExampleGroup,
                    ExampleMessage,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }
}
