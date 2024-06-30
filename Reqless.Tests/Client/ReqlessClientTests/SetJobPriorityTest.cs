using Reqless.Client;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetJobPriorityAsync"/>.
/// </summary>
public class SetJobPriorityTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should throw if jid is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetJobPriorityAsync(
                    jid: null!,
                    priority: 21
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        var newPriority = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            async subject =>
            {
                var updatedSuccessfully = await subject.SetJobPriorityAsync(
                    jid: ExampleJid,
                    priority: newPriority
                );
                Assert.True(updatedSuccessfully);
            },
            expectedArguments: [
                "job.setPriority",
                0,
                ExampleJid,
                newPriority,
            ],
            returnValue: true
        );
    }
}
