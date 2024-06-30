using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.CancelJobsAsync"/>.
/// </summary>
public class CancelJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                bool cancelledSuccessfully = await subject.CancelJobsAsync(
                    ExampleJid,
                    ExampleJidOther
                );
                Assert.True(cancelledSuccessfully);
            },
            expectedArguments: [
                "job.cancel",
                0,
                ExampleJid,
                ExampleJidOther,
            ],
            returnValues: [ExampleJid, ExampleJidOther]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> returns true if no jids are
    /// given.
    /// </summary>
    [Fact]
    public async void ReturnsTrueIfNoJidsAreGiven()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                bool cancelledSuccessfully = await subject.CancelJobsAsync();
                Assert.True(cancelledSuccessfully);
            }
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> throws if any of the jids
    /// are null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidsAreNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelJobsAsync(ExampleJid, null!)
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'jids')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> throws if any of the jids
    /// are empty or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidsAreEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.CancelJobsAsync(ExampleJid, null!)
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'jids')",
                exception.Message
            );
        }
    }
}
