using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should throw if jid is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.SetJobPriorityAsync(
                        jid: emptyString,
                        priority: 21
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetJobPriorityAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        var newPriority = 42;
        var updatedSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetJobPriorityAsync(
                jid: ExampleJid,
                priority: newPriority
            ),
            expectedArguments: [
                "job.setPriority",
                0,
                ExampleJid,
                newPriority,
            ],
            returnValue: true
        );
        Assert.True(updatedSuccessfully);
    }
}