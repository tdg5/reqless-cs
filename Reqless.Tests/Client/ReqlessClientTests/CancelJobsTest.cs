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
        bool cancelledSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CancelJobsAsync(
                ExampleJid,
                ExampleJidOther
            ),
            expectedArguments: [
                "job.cancel",
                0,
                ExampleJid,
                ExampleJidOther,
            ],
            returnValues: [ExampleJid, ExampleJidOther]
        );
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> returns true if no jids are
    /// given.
    /// </summary>
    [Fact]
    public async void ReturnsTrueIfNoJidsAreGiven()
    {
        bool cancelledSuccessfully = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.CancelJobsAsync()
        );
        Assert.True(cancelledSuccessfully);
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> should throw if jids
    /// argument is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidsArgumentIsNull()
    {
        await Scenario.ThrowsArgumentNullExceptionAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelJobsAsync(null!)
            ),
            "jids"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.CancelJobsAsync"/> throws if any of the jids
    /// are null, empty, or composed entirely of whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidsAreNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterItemIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.CancelJobsAsync(ExampleJid, invalidJid!)
            ),
            "jids"
        );
    }
}