using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetThrottleAsync"/>.
/// </summary>
public class SetThrottleTest : BaseReqlessClientTest
{
    private readonly int exampleTtl = 42;

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should throw if throttle
    /// name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottleName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetThrottleAsync(
                    maximum: 21, throttleName: invalidThrottleName!)),
            "throttleName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should throw if
    /// maximum is negative.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfMaximumIsLessThanZero()
    {
        await Scenario.ThrowsWhenArgumentIsNegativeAsync(
            (invalidMaximum) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetThrottleAsync(
                    throttleName: ExampleThrottleName, maximum: invalidMaximum)),
            "maximum");
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetThrottleAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var newMaximum = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetThrottleAsync(
                maximum: newMaximum,
                throttleName: ExampleThrottleName,
                ttl: exampleTtl),
            expectedArguments: [
                "throttle.set",
                0,
                ExampleThrottleName,
                newMaximum,
                exampleTtl,
            ]);
    }
}
