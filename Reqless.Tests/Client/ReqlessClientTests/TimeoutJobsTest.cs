using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.TimeoutJobsAsync"/>.
/// </summary>
public class TimeoutJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TimeoutJobsAsync(ExampleJid, ExampleJidOther),
            expectedArguments: ["job.timeout", 0, ExampleJid, ExampleJidOther]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should throw if the given
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TimeoutJobsAsync(null!, ExampleJidOther),
                expectedArguments: ["job.timeout", 0, ExampleJid, ExampleJidOther]
            )
        );
        Assert.Equal(
            "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'jids')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should throw if the given
    /// jid is empty or only whitespcae.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.TimeoutJobsAsync(emptyString, ExampleJidOther),
                    expectedArguments: ["job.timeout", 0, ExampleJid, ExampleJidOther]
                )
            );
            Assert.Equal(
                "Value cannot include null, empty string, or strings composed entirely of whitespace. (Parameter 'jids')",
                exception.Message
            );
        }
    }
}