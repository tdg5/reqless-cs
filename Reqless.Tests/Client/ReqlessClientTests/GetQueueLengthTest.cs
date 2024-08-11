using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetQueueLengthAsync"/>.
/// </summary>
public class GetQueueLengthTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should throw if queue
    /// name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueLengthAsync(queueName: invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueLengthAsync(ExampleQueueName),
                expectedArguments: [
                    "queue.length",
                    0,
                    ExampleQueueName,
                ],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        int expectedLength = 5;
        int actualLength = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetQueueLengthAsync(ExampleQueueName),
            expectedArguments: [
                "queue.length",
                0,
                ExampleQueueName,
            ],
            returnValue: expectedLength
        );
        Assert.Equal(expectedLength, actualLength);
    }
}
