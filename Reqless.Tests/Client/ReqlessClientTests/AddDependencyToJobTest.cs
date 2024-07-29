using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.AddDependencyToJobAsync"/>.
/// </summary>
public class AddDependencyToJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if jid
    /// is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: ExampleJid,
                    jid: invalidJid!
                )
            ),
            "jid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if
    /// dependsOnJid is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfDependsOnJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidDependsOnJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: invalidDependsOnJid!,
                    jid: ExampleJid
                )
            ),
            "dependsOnJid"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should call
    /// Executor with the expected
    /// arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var otherJid = "otherJid";
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: otherJid,
                    jid: ExampleJid
                ),
            expectedArguments: [
                "job.addDependency",
                0,
                ExampleJid,
                otherJid
            ],
            returnValue: true
        );
        Assert.True(result);
    }
}
