using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetJobPriorityAsync"/>.
/// </summary>
public class SetJobPriorityTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should throw if jid is
    /// null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetJobPriorityAsync(jid: invalidJid!, priority: 21)),
            "jid");
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var newPriority = 42;
        var updatedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetJobPriorityAsync(
                jid: ExampleJid, priority: newPriority),
            expectedArguments: [
                "job.setPriority",
                0,
                ExampleJid,
                newPriority,
            ],
            returnValue: true);
        Assert.True(updatedSuccessfully);
    }
}
