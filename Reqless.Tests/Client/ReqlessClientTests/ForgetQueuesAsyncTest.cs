using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ForgetQueuesAsync"/>.
/// </summary>
public class ForgetQueuesTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> throws if the given
    /// collection is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNamesIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetQueuesAsync(null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueNames')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> throws if any of the given
    /// queue names are null, empty string, or strings composed entirely of
    /// whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyQueueNameIsNullEmptyOrOnlyWhitespace()
    {
        foreach (var invalidQueueName in TestConstants.EmptyStringsWithNull)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.ForgetQueuesAsync(invalidQueueName!)
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'queueNames')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> calls the executor with the
    /// expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        var otherQueue = "other-queue";
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ForgetQueuesAsync(ExampleQueueName, otherQueue),
            expectedArguments: ["queue.forget", 0, ExampleQueueName, otherQueue]
        );
    }
}