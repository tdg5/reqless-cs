using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: invalidJid!, workerName: ExampleWorkerName)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should throw if the
    /// given worker name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: ExampleJid, workerName: invalidWorkerName!)),
            "workerName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should throw if the
    /// given data is empty or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDataIsEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsEmptyOrWhitespaceAsync(
            (invalidData) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    data: invalidData, jid: ExampleJid, workerName: ExampleWorkerName)),
            "data");
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ReturnsTheNewExpirationTimeWithoutData()
    {
        var expectedExpiration = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 60000;
        long newExpiration = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.HeartbeatJobAsync(
                jid: ExampleJid, workerName: ExampleWorkerName),
            expectedArguments: [
                "job.heartbeat",
                0,
                ExampleJid,
                ExampleWorkerName,
            ],
            returnValue: expectedExpiration);
        Assert.Equal(expectedExpiration, newExpiration);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> should return the new
    /// expiration time.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ReturnsTheNewExpirationTimeWithData()
    {
        var expectedExpiration = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 60000;
        long newExpiration = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.HeartbeatJobAsync(
                data: ExampleData,
                jid: ExampleJid,
                workerName: ExampleWorkerName),
            expectedArguments: [
                "job.heartbeat",
                0,
                ExampleJid,
                ExampleWorkerName,
                ExampleData,
            ],
            returnValue: expectedExpiration);
        Assert.Equal(expectedExpiration, newExpiration);
    }

    /// <summary>
    /// <see cref="ReqlessClient.HeartbeatJobAsync"/> throws if the expiration
    /// time is unexpected.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfExpirationTimeIsUnexpected()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.HeartbeatJobAsync(
                    jid: ExampleJid, workerName: ExampleWorkerName),
                expectedArguments: [
                    "job.heartbeat",
                    0,
                    ExampleJid,
                    ExampleWorkerName,
                ],
                returnValue: null));
    }
}
