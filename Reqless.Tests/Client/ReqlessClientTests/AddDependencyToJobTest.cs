using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: ExampleJid,
                    jid: invalidJid!)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should throw if
    /// dependsOnJid is null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfDependsOnJidIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidDependsOnJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.AddDependencyToJobAsync(
                    dependsOnJid: invalidDependsOnJid!,
                    jid: ExampleJid)),
            "dependsOnJid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.AddDependencyToJobAsync"/> should call
    /// Executor with the expected
    /// arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var otherJid = "otherJid";
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.AddDependencyToJobAsync(
                dependsOnJid: otherJid, jid: ExampleJid),
            expectedArguments: ["job.addDependency", 0, ExampleJid, otherJid],
            returnValue: true);
        Assert.True(result);
    }
}
