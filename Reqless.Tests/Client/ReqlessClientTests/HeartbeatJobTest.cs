using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.HeartbeatJobAsync"/>.
/// </summary>
public class HeartbeatJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should throw if the
    /// given job ID is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: invalidJid!,
                    workerName: ExampleWorkerName
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: ExampleJid,
                    workerName: invalidWorkerName!
                )
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should throw if the
    /// given data is empty or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfDataIsEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    data: invalidData,
                    jid: ExampleJid,
                    workerName: ExampleWorkerName
                )
            ),
            "data"
        );
    }

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