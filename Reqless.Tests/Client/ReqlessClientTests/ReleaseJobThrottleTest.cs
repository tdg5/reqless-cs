using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ReleaseJobThrottleAsync"/>.
/// </summary>
public class ReleaseJobThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ReleaseJobThrottleAsync"/> should call the
    /// executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ReleaseJobThrottleAsync(
                ExampleJid,
                ExampleThrottleName
            ),
            expectedArguments: [
                "throttle.release",
                0,
                ExampleThrottleName,
                ExampleJid,
            ]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseJobThrottleAsync"/> throws if the
    /// given jid argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ReleaseJobThrottleAsync(
                    invalidJid!,
                    ExampleThrottleName
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseJobThrottleAsync"/> throws if the
    /// given throttleName argument is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottle) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ReleaseJobThrottleAsync(
                    ExampleJid,
                    invalidThrottle!
                )
            ),
            "throttleName"
        );
    }
}