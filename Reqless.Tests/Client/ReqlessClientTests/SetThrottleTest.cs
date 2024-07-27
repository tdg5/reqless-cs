using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetThrottleAsync"/>.
/// </summary>
public class SetThrottleTest : BaseReqlessClientTest
{
    private readonly int ExampleTtl = 42;

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should throw if throttle
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfThrottleNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetThrottleAsync(
                    maximum: 21,
                    throttleName: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'throttleName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfThrottleNameIsEmptyOrJustWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.SetThrottleAsync(
                        maximum: 21,
                        throttleName: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'throttleName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should throw if
    /// maximum is negative.
    /// </summary>
    [Fact]
    public async void ThrowsIfMaximumIsLessThanZero()
    {
        foreach (var invalidMaximum in new int[] { -100, -1 })
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.SetThrottleAsync(
                        throttleName: ExampleThrottleName,
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
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        var newMaximum = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetThrottleAsync(
                maximum: newMaximum,
                throttleName: ExampleThrottleName,
                ttl: ExampleTtl
            ),
            expectedArguments: [
                "throttle.set",
                0,
                ExampleThrottleName,
                newMaximum,
                ExampleTtl,
            ]
        );
    }
}