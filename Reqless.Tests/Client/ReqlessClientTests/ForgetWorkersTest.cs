using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ForgetWorkersAsync"/>.
/// </summary>
public class ForgetWorkersTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ForgetWorkersAsync"/> throws if the given
    /// collection is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfWorkerNamesIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetWorkersAsync(null!)
            ),
            "workerNames"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetWorkersAsync"/> throws if any of the given
    /// worker names are null, empty string, or strings composed entirely of
    /// whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyWorkerNameIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidWorkerName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetWorkersAsync(invalidWorkerName!)
            ),
            "workerNames"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetWorkersAsync"/> calls the executor with the
    /// expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var otherWorker = "other-worker";
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ForgetWorkersAsync(ExampleWorkerName, otherWorker),
            expectedArguments: ["worker.forget", 0, ExampleWorkerName, otherWorker]
        );
    }
}
