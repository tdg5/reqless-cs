using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetQueueThrottleAsync"/>.
/// </summary>
public class SetQueueThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if queue
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetQueueThrottleAsync(
                    queueName: null!,
                    maximum: 21
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrJustWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.SetQueueThrottleAsync(
                        queueName: emptyString,
                        maximum: 21
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if
    /// maximum is negative.
    /// </summary>
    [Fact]
    public async void ThrowsIfMaximumIsLessThanZero()
    {
        foreach (var invalidMaximum in new int[] { -100, -1 })
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.SetQueueThrottleAsync(
                        queueName: ExampleQueueName,
                        maximum: invalidMaximum
                    )
                )
            );
            Assert.Equal(
                "Value must be greater than or equal to zero. (Parameter 'maximum')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        var newMaximum = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetQueueThrottleAsync(
                queueName: ExampleQueueName,
                maximum: newMaximum
            ),
            expectedArguments: [
                "queue.throttle.set",
                0,
                ExampleQueueName,
                newMaximum,
            ]
        );
    }
}