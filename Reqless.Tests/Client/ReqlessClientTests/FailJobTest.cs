using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.FailJobAsync"/>.
/// </summary>
public class FailJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should call Executor with
    /// the expected arguments when data is given.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArgumentsWhenGivenData()
    {
        bool failedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.FailJobAsync(
                data: ExampleData,
                group: ExampleGroup,
                jid: ExampleJid,
                message: ExampleMessage,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.fail",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleGroup,
                ExampleMessage,
                ExampleData
            ],
            returnValue: ExampleJid
        );
        Assert.True(failedSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.FailJobAsync"/> should call Executor with
    /// the expected arguments when data is not given.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArgumentsWhenNotGivenData()
    {
        bool failedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.FailJobAsync(
                group: ExampleGroup,
                jid: ExampleJid,
                message: ExampleMessage,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.fail",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleGroup,
                ExampleMessage
            ],
            returnValue: ExampleJid
        );
        Assert.True(failedSuccessfully);
    }
}