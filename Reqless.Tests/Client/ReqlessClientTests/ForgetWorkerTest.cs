using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ForgetWorkerAsync"/>.
/// </summary>
public class ForgetWorkerTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ForgetWorkerAsync"/> throws if the given worker
    /// name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetWorkerAsync(invalidWorkerName!)
            ),
            "workerName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetWorkerAsync"/> calls the executor with the
    /// expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ForgetWorkerAsync(ExampleWorkerName),
            expectedArguments: ["worker.forget", 0, ExampleWorkerName]
        );
    }
}
