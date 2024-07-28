using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CancelRecurringJobAsync"/>.
/// </summary>
public class CancelRecurringJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CancelRecurringJobAsync"/> should throw if the
    /// given jid is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelRecurringJobAsync(jid: invalidJid!)
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelRecurringJobAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CancelRecurringJobAsync(jid: ExampleJid),
            expectedArguments: ["recurringJob.cancel", 0, ExampleJid],
            returnValue: ExampleJid
        );
    }
}
