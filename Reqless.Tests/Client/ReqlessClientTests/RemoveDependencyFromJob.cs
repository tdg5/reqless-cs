using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/>.
/// </summary>
public class RemoveDependencyFromJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// jid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveDependencyFromJobAsync(
                    dependsOnJid: ExampleJid,
                    jid: invalidJid!
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should throw if
    /// dependsOnJid is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDependsOnJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidDependsOnJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.RemoveDependencyFromJobAsync(
                    dependsOnJid: invalidDependsOnJid!,
                    jid: ExampleJid
                )
            ),
            "dependsOnJid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.RemoveDependencyFromJobAsync"/> should call
    /// Executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        bool result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.RemoveDependencyFromJobAsync(
                dependsOnJid: ExampleJidOther,
                jid: ExampleJid
            ),
            expectedArguments: [
                "job.removeDependency",
                0,
                ExampleJid,
                ExampleJidOther,
            ],
            returnValue: true
        );
        Assert.True(result);
    }
}
