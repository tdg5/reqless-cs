using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.HeartbeatJobAsync"/>.
/// </summary>
public class HeartbeatJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time.
    /// </summary>
    [Fact]
    public async void ReturnsTheNewExpirationTimeWithoutData()
    {
        var expectedExpiration = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 60000;
        long newExpiration = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.HeartbeatJobAsync(
                jid: ExampleJid,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.heartbeat",
                0,
                ExampleJid,
                ExampleWorkerName,
            ],
            returnValue: expectedExpiration
        );
        Assert.Equal(expectedExpiration, newExpiration);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time.
    /// </summary>
    [Fact]
    public async void ReturnsTheNewExpirationTimeWithData()
    {
        var expectedExpiration = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 60000;
        long newExpiration = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.HeartbeatJobAsync(
                data: ExampleData,
                jid: ExampleJid,
                workerName: ExampleWorkerName
            ),
            expectedArguments: [
                "job.heartbeat",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleData,
            ],
            returnValue: expectedExpiration
        );
        Assert.Equal(expectedExpiration, newExpiration);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> throws if the expiration
    /// time is unexpected.
    /// </summary>
    [Fact]
    public async void ThrowsIfExpirationTimeIsUnexpected()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: ExampleJid,
                    workerName: ExampleWorkerName
                ),
                expectedArguments: [
                    "job.heartbeat",
                    0,
                    ExampleJid,
                    ExampleWorkerName,
                ],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }
}