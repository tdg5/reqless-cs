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
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetQueueLengthAsync(queueName: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetQueueLengthAsync(emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
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
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetQueueLengthAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async void ReturnsValidResultFromTheServer()
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